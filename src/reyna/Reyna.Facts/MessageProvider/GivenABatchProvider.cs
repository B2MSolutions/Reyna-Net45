namespace Reyna.Facts
{
    using System;
    using System.Collections.Generic;
    using Moq;
    using Interfaces;
    using Xunit;

    public class GivenABatchProvider
    {
        private const long IntervalDay = 24 * 60 * 60 * 1000;

        public GivenABatchProvider()
        {
            Repository = new Mock<IRepository>();
            BatchConfiguration = new Mock<IBatchConfiguration>();
            PeriodicBackoutCheck = new Mock<IPeriodicBackoutCheck>();

            BatchConfiguration.SetupGet(b => b.BatchMessageCount).Returns(3);
            BatchConfiguration.SetupGet(b => b.BatchMessagesSize).Returns(1000);
            BatchConfiguration.SetupGet(b => b.BatchUrl).Returns(new Uri("http://post.com/api/batch"));
            BatchConfiguration.SetupGet(b => b.SubmitInterval).Returns(24 * 60 * 60 * 1000);

            Provider = new BatchProvider(Repository.Object, PeriodicBackoutCheck.Object, BatchConfiguration.Object);
        }

        private Mock<IRepository> Repository { get; set; }

        private Mock<IBatchConfiguration> BatchConfiguration { get; set; }

        private Mock<IPeriodicBackoutCheck> PeriodicBackoutCheck { get; set; }

        private BatchProvider Provider { get; set; }

        [Fact]
        public void WhenConstructingShouldNotThrow()
        {
            Assert.NotNull(Provider);
            Assert.NotNull(BatchConfiguration);
        }

        [Fact]
        public void WhenCallingCanSendShouldRerurnTrue()
        {
            Assert.False(Provider.CanSend);
        }

        [Fact]
        public void WhenCallingDeleteShouldRemoveMessage()
        {
            var messge = new Mock<IMessage>();
            Provider.Delete(messge.Object);

            Repository.Verify(r => r.DeleteMessagesFrom(messge.Object));
        }

        [Fact]
        public void WhenCallingGetNextAndNoMessageShouldReturnNull()
        {
            Repository.Setup(r => r.Get()).Returns((IMessage)null);

            var actual = Provider.GetNext();

            Assert.Null(actual);
        }

        [Fact]
        public void WhenCallingGetNextShouldReturnCorrectFormat()
        {
            var messages = GetTestMessages();
            Repository.Setup(r => r.Get()).Returns(messages[0]);
            Repository.Setup(r => r.GetNextMessageAfter(1))
                .Returns(messages[1]);
            Repository.Setup(r => r.GetNextMessageAfter(2))
                .Returns(messages[2]);
            Repository.Setup(r => r.GetNextMessageAfter(3))
                .Returns((IMessage)null);

            var actual = Provider.GetNext();

            Assert.NotNull(actual);

            Assert.Equal("http://post.com/api/batch", actual.Url.AbsoluteUri);
            Assert.Equal(
                "{\"events\":[" +
                    "{\"url\":\"http://google.com/\", \"reynaId\":1, \"payload\":{\"key01\":\"value01\",\"key02\":11}}, " +
                    "{\"url\":\"http://google2.com/\", \"reynaId\":2, \"payload\":{\"key11\":\"value11\",\"key12\":12}}, " +
                    "{\"url\":\"http://google3.com/\", \"reynaId\":3, \"payload\":{\"key21\":\"value21\",\"key22\":22}}" +
                    "]}",
                    actual.Body);

            AssertHeaders(actual);
            Assert.Equal(3, actual.Id);
        }

        [Fact]
        public void WhenCallingGetNextAndThereIsCorruptedMessageShouldPostIt()
        {
            Message message = new Message(new Uri("http://google2.com"), "{\"body\":\"{\\\"key11\\\":\"}");
            message.Id = 2;
            AddHeaders(message);

            var messages = GetTestMessages();
            Repository.Setup(r => r.Get()).Returns(messages[0]);
            Repository.Setup(r => r.GetNextMessageAfter(1))
                .Returns(message);
            Repository.Setup(r => r.GetNextMessageAfter(2))
                .Returns(messages[2]);
            Repository.Setup(r => r.GetNextMessageAfter(3))
                .Returns((IMessage)null);

            var actual = Provider.GetNext();

            Assert.NotNull(actual);

            Assert.Equal("http://post.com/api/batch", actual.Url.AbsoluteUri);
            Assert.Equal(
                "{\"events\":[" +
                    "{\"url\":\"http://google.com/\", \"reynaId\":1, \"payload\":{\"key01\":\"value01\",\"key02\":11}}, " +
                    "{\"url\":\"http://google2.com/\", \"reynaId\":2, \"payload\":{\"body\":\"{\\\"key11\\\":\"}}, " +
                    "{\"url\":\"http://google3.com/\", \"reynaId\":3, \"payload\":{\"key21\":\"value21\",\"key22\":22}}" +
                    "]}",
                    actual.Body);

            AssertHeaders(actual);
            Assert.Equal(3, actual.Id);
        }

        [Fact]
        public void WhenCallingGetNextShouldReturnMessagesRelatedToMaximumLimit()
        {
            BatchConfiguration.SetupGet(b => b.BatchMessageCount).Returns(2);

            var messages = GetTestMessages();
            Repository.Setup(r => r.Get()).Returns(messages[0]);
            Repository.Setup(r => r.GetNextMessageAfter(1))
                .Returns(messages[1]);
            Repository.Setup(r => r.GetNextMessageAfter(2))
                .Returns(messages[2]);
            Repository.Setup(r => r.GetNextMessageAfter(3))
                .Returns((IMessage)null);

            var actual = Provider.GetNext();

            Assert.NotNull(actual);

            Assert.Equal("http://post.com/api/batch", actual.Url.AbsoluteUri);
            Assert.Equal(
                "{\"events\":[" +
                    "{\"url\":\"http://google.com/\", \"reynaId\":1, \"payload\":{\"key01\":\"value01\",\"key02\":11}}, " +
                    "{\"url\":\"http://google2.com/\", \"reynaId\":2, \"payload\":{\"key11\":\"value11\",\"key12\":12}}" +
                    "]}",
                    actual.Body);

            AssertHeaders(actual);
            Assert.Equal(2, actual.Id);
        }

        [Fact]
        public void WhenCallingGetNextShouldReturnMessagesRelatedToMaximumSize()
        {
            BatchConfiguration.SetupGet(b => b.BatchMessagesSize).Returns(100);
            var messages = GetTestMessages();
            Repository.Setup(r => r.Get()).Returns(messages[0]);
            Repository.Setup(r => r.GetNextMessageAfter(1))
                .Returns(messages[1]);
            Repository.Setup(r => r.GetNextMessageAfter(2))
                .Returns(messages[2]);
            Repository.Setup(r => r.GetNextMessageAfter(3))
                .Returns((IMessage)null);

            var actual = Provider.GetNext();

            Assert.NotNull(actual);

            Assert.Equal("http://post.com/api/batch", actual.Url.AbsoluteUri);
            Assert.Equal(
                "{\"events\":[" +
                    "{\"url\":\"http://google.com/\", \"reynaId\":1, \"payload\":{\"key01\":\"value01\",\"key02\":11}}" +
                    "]}",
                    actual.Body);

            AssertHeaders(actual);
            Assert.Equal(1, actual.Id);
        }

        [Fact]
        public void WhenCallingGetNextAndNoUrlConfiguredShouldReturnUrlWithBatchAppended()
        {
            BatchConfiguration.SetupGet(b => b.BatchMessagesSize).Returns(95);
            BatchConfiguration.SetupGet(b => b.BatchUrl).Returns((Uri)null);
            var messages = GetTestMessages();
            Repository.Setup(r => r.Get()).Returns(messages[0]);
            Repository.Setup(r => r.GetNextMessageAfter(1))
                .Returns(messages[1]);
            Repository.Setup(r => r.GetNextMessageAfter(2))
                .Returns(messages[2]);
            Repository.Setup(r => r.GetNextMessageAfter(3))
                .Returns((IMessage)null);

            var actual = Provider.GetNext();

            Assert.NotNull(actual);
            Assert.Equal("http://google.com/batch", actual.Url.ToString());
        }

        [Fact]
        public void WhenCallingGetNextAndNoUrlConfiguredAndHTTPSShouldReturnUrlWithBatchAppended()
        {
            BatchConfiguration.SetupGet(b => b.BatchMessagesSize).Returns(95);
            BatchConfiguration.SetupGet(b => b.BatchUrl).Returns((Uri)null);

            var messages = GetTestMessages();
            Repository.Setup(r => r.Get()).Returns(messages[0]);
            Repository.Setup(r => r.GetNextMessageAfter(1))
                .Returns(messages[1]);
            Repository.Setup(r => r.GetNextMessageAfter(2))
                .Returns(messages[2]);
            Repository.Setup(r => r.GetNextMessageAfter(3))
                .Returns((IMessage)null);

            ((Message)messages[0]).Url = new Uri("https://www.post.com/1/2/req");
            ((Message)messages[1]).Url = new Uri("https://www.post.com/1/2/req");
            ((Message)messages[2]).Url = new Uri("https://www.post.com/1/2/req");

            var actual = Provider.GetNext();

            Assert.NotNull(actual);
            Assert.Equal("https://www.post.com/1/2/batch", actual.Url.ToString());
        }

        [Fact]
        public void WhenCallingCanSendAndTimeNotElapsedShouldReturnFalse()
        {
            long interval = (long)(IntervalDay * 0.9);
            PeriodicBackoutCheck.Setup(p => p.IsTimeElapsed("BatchProvider", interval)).Returns(false);

            var actual = Provider.CanSend;

            Assert.False(actual);
            PeriodicBackoutCheck.Verify(p => p.IsTimeElapsed("BatchProvider", interval), Times.Once());
        }

        [Fact]
        public void WhenCallingCanSendAndTimeElapsedShouldReturnTrue()
        {
            long interval = (long)(IntervalDay * 0.9);
            PeriodicBackoutCheck.Setup(p => p.IsTimeElapsed("BatchProvider", interval)).Returns(true);

            var actual = Provider.CanSend;

            Assert.True(actual);
            PeriodicBackoutCheck.Verify(p => p.IsTimeElapsed("BatchProvider", interval), Times.Once());
        }

        [Fact]
        public void WhenCallingCanSendThereAreMoreMessagesThanMaxMessagesCountShouldSend()
        {
            long interval = (long)(IntervalDay * 0.9);
            PeriodicBackoutCheck.Setup(p => p.IsTimeElapsed("BatchProvider", interval)).Returns(false);
            BatchConfiguration.SetupGet(b => b.BatchMessageCount).Returns(100);
            Repository.SetupGet(r => r.AvailableMessagesCount).Returns(100);

            var actual = Provider.CanSend;

            Assert.True(actual);
            PeriodicBackoutCheck.Verify(p => p.IsTimeElapsed("BatchProvider", interval), Times.Once());
            BatchConfiguration.Verify(p => p.BatchMessageCount, Times.Once());
            Repository.Verify(p => p.AvailableMessagesCount, Times.Once());
        }

        [Fact]
        public void WhenCallingCanSendThereAreLessMessagesThanMaxMessagesCountShouldSend()
        {
            long interval = (long)(IntervalDay * 0.9);
            PeriodicBackoutCheck.Setup(p => p.IsTimeElapsed("BatchProvider", interval)).Returns(false);
            BatchConfiguration.SetupGet(b => b.BatchMessageCount).Returns(100);
            Repository.SetupGet(r => r.AvailableMessagesCount).Returns(99);

            var actual = Provider.CanSend;

            Assert.False(actual);
            PeriodicBackoutCheck.Verify(p => p.IsTimeElapsed("BatchProvider", interval), Times.Once());
            BatchConfiguration.Verify(p => p.BatchMessageCount, Times.Once());
            Repository.Verify(p => p.AvailableMessagesCount, Times.Once());
        }

        [Fact]
        public void WhenCallingCloseAndNeverSendSuccessfulBatchShouldNotRecord()
        {
            Provider.Close();

            PeriodicBackoutCheck.Verify(p => p.Record("BatchProvider"), Times.Never());
        }

        [Fact]
        public void WhenCallingCloseAndSuccessfullySentBatchShouldRecord()
        {
            var message = new Message(new Uri("http://google.com"), "{\"key01\":\"value01\",\"key02\":11}");
            message.Id = 1;

            Provider.Delete(message);
            Provider.Close();

            PeriodicBackoutCheck.Verify(p => p.Record("BatchProvider"), Times.Once());
        }

        [Fact]
        public void WhenCallingDeleteShouldDeleteFromRepository()
        {
            var message = new Message(new Uri("http://google.com"), "{\"key01\":\"value01\",\"key02\":11}");
            message.Id = 100;

            Provider.Delete(message);

            Repository.Verify(p => p.DeleteMessagesFrom(message), Times.Once());
        }

        private List<IMessage> GetTestMessages()
        {
            var message1 = new Message(new Uri("http://google.com"), "{\"key01\":\"value01\",\"key02\":11}");
            var message2 = new Message(new Uri("http://google2.com"), "{\"key11\":\"value11\",\"key12\":12}");
            var message3 = new Message(new Uri("http://google3.com"), "{\"key21\":\"value21\",\"key22\":22}");

            message1.Id = 1;
            message2.Id = 2;
            message3.Id = 3;

            AddHeaders(message1);
            AddHeaders(message2);
            AddHeaders(message3);

            List<IMessage> messages = new List<IMessage>(3);
            messages.Add(message1);
            messages.Add(message2);
            messages.Add(message3);

            return messages;
        }

        private void AddHeaders(IMessage message)
        {
            message.Headers.Add("key1", "value1");
            message.Headers.Add("key2", "value2");
            message.Headers.Add("key4", "value4");
        }

        private void AssertHeaders(IMessage actual)
        {
            Assert.Equal(3, actual.Headers.Count);
            Assert.Equal("key1", actual.Headers.Keys[0]);
            Assert.Equal("value1", actual.Headers.Get(0));
            Assert.Equal("key2", actual.Headers.Keys[1]);
            Assert.Equal("value2", actual.Headers.Get(1));
            Assert.Equal("key4", actual.Headers.Keys[2]);
            Assert.Equal("value4", actual.Headers.Get(2));
        }
    }
}
