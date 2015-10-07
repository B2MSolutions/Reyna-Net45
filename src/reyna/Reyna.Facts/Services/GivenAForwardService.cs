namespace Reyna.Facts
{
    using System;
    using System.IO;
    using System.Reflection;
    using System.Threading;
    using Moq;
    using Reyna.Interfaces;
    using Xunit;
    using System.Collections.Generic;

    public class GivenAForwardService
    {
        private ForwardService service;
        private Mock<IAutoResetEventAdapter> waitHandle;
        private Mock<IHttpClient> httpClient;
        private Mock<INetworkStateService> networkStateService;
        private Mock<IRepository> persistentStore;

        public GivenAForwardService()
        {
            this.waitHandle = new Mock<IAutoResetEventAdapter>();
            this.httpClient = new Mock<IHttpClient>();
            this.networkStateService = new Mock<INetworkStateService>();
            this.persistentStore = new Mock<IRepository>();

            this.service = new ForwardService(this.waitHandle.Object);
        }

        [Fact]
        public void whenCallingInitialiseShouldInitialiseClassCorrectly()
        {
            this.service.Initialize(this.persistentStore.Object, this.httpClient.Object, this.networkStateService.Object, 10000, 5000);

            Assert.Equal(this.persistentStore.Object, this.service.SourceStore);
            Assert.Equal(this.httpClient.Object, this.service.HttpClient);
            Assert.Equal(this.networkStateService.Object, this.service.NetworkState);
            Assert.Equal(5000, this.service.SleepMilliseconds);
            Assert.Equal(10000, this.service.TemporaryErrorMilliseconds);
        }

        [Fact]
        public void whenCallingInitialiseWithNullHttpClientShouldThrow()
        {
            Assert.Throws<ArgumentNullException>(() => this.service.Initialize(this.persistentStore.Object, null, this.networkStateService.Object, 10000, 5000));
        }

        [Fact]
        public void whenCallingInitialiseWithNullNullNetworkStateServiceShouldThrow()
        {
            Assert.Throws<ArgumentNullException>(() => this.service.Initialize(this.persistentStore.Object, this.httpClient.Object, null, 10000, 5000));
        }

        [Fact]
        public void whenCallingInitialiseWithNullNullSourceStoreShouldThrow()
        {
            Assert.Throws<ArgumentNullException>(() => this.service.Initialize(null, this.httpClient.Object, this.networkStateService.Object, 10000, 5000));
        }

        [Fact]
        public void whenCallingStartShouldCallWaitHandle()
        {
            this.service.Initialize(this.persistentStore.Object, this.httpClient.Object, this.networkStateService.Object, 10000, 5000);

            this.service.Start();
            Thread.Sleep(50);
            this.service.Stop();

            this.waitHandle.Verify(w => w.WaitOne(), Times.AtLeastOnce());
            this.waitHandle.Verify(w => w.Reset(), Times.AtLeastOnce());
        }

        [Fact]
        public void whenCallingStartAndThereIsAMessageShouldCallSendOnHttpClientAndRemoveTheMessage()
        {
            this.service.Initialize(this.persistentStore.Object, this.httpClient.Object, this.networkStateService.Object, 10000, 5000);
            Message message = new Message(new Uri("http://google.com"), "MessageBody");
            List<IMessage> messages = new List<IMessage> { message };

            this.persistentStore.Setup(v => v.Get()).Returns(() => this.GetMessage(messages));
            this.persistentStore.Setup(v => v.Remove()).Callback(() => this.RemoveMessage(messages, message));

            this.service.DoWork();

            this.httpClient.Verify(h => h.Post(message), Times.Exactly(1));
            Assert.Equal(0, messages.Count);
        }

        [Fact]
        public void whenCallingDoWorkAndHttpClientReturnsTemporaryErrorShouldNotRemoveMessage()
        {
            this.service.Initialize(this.persistentStore.Object, this.httpClient.Object, this.networkStateService.Object, 0, 0);
            Message message = new Message(new Uri("http://google.com"), "MessageBody");
            List<IMessage> messages = new List<IMessage> { message };

            this.persistentStore.Setup(v => v.Get()).Returns(() => this.GetMessage(messages));
            this.persistentStore.Setup(v => v.Remove()).Callback(() => this.RemoveMessage(messages, message));
            this.httpClient.Setup(h => h.Post(It.IsAny<IMessage>())).Returns(Result.TemporaryError);

            this.service.DoWork();

            this.httpClient.Verify(h => h.Post(message), Times.Exactly(1));
            this.waitHandle.Verify(w => w.Reset(), Times.Exactly(1));
            this.persistentStore.Verify(p => p.Remove(), Times.Never);
            Assert.Equal(1, messages.Count);
        }

        [Fact]
        public void whenCallingDoWorkAndHttpClientReturnsblackoutShouldNotRemoveMessage()
        {
            this.service.Initialize(this.persistentStore.Object, this.httpClient.Object, this.networkStateService.Object, 0, 0);
            Message message = new Message(new Uri("http://google.com"), "MessageBody");
            List<IMessage> messages = new List<IMessage> { message };

            this.persistentStore.Setup(v => v.Get()).Returns(() => this.GetMessage(messages));
            this.persistentStore.Setup(v => v.Remove()).Callback(() => this.RemoveMessage(messages, message));
            this.httpClient.Setup(h => h.Post(It.IsAny<IMessage>())).Returns(Result.Blackout);

            this.service.DoWork();

            this.httpClient.Verify(h => h.Post(message), Times.Exactly(1));
            this.waitHandle.Verify(w => w.Reset(), Times.Exactly(1));
            this.persistentStore.Verify(p => p.Remove(), Times.Never);
            Assert.Equal(1, messages.Count);
        }

        [Fact]
        public void whenCallingDoWorkAndHttpClientReturnsNotConnectedShouldNotRemoveMessage()
        {
            this.service.Initialize(this.persistentStore.Object, this.httpClient.Object, this.networkStateService.Object, 0, 0);
            Message message = new Message(new Uri("http://google.com"), "MessageBody");
            List<IMessage> messages = new List<IMessage> { message };

            this.persistentStore.Setup(v => v.Get()).Returns(() => this.GetMessage(messages));
            this.persistentStore.Setup(v => v.Remove()).Callback(() => this.RemoveMessage(messages, message));
            this.httpClient.Setup(h => h.Post(It.IsAny<IMessage>())).Returns(Result.NotConnected);

            this.service.DoWork();

            this.httpClient.Verify(h => h.Post(message), Times.Exactly(1));
            this.waitHandle.Verify(w => w.Reset(), Times.Exactly(1));
            this.persistentStore.Verify(p => p.Remove(), Times.Never);
            Assert.Equal(1, messages.Count);
        }

        [Fact]
        public void whenCallingDoWorkAndServiceIsTerminatingShouldNotSendMessage()
        {
            this.service.Initialize(this.persistentStore.Object, this.httpClient.Object, this.networkStateService.Object, 0, 0);
            Message message = new Message(new Uri("http://google.com"), "MessageBody");
            List<IMessage> messages = new List<IMessage> { message };

            this.persistentStore.Setup(v => v.Get()).Returns(() => this.GetMessage(messages));
            this.persistentStore.Setup(v => v.Remove()).Callback(() => this.RemoveMessage(messages, message));

            this.service.Terminate = true;
            this.service.DoWork();

            this.httpClient.Verify(h => h.Post(message), Times.Never());
            this.waitHandle.Verify(w => w.Reset(), Times.Exactly(1));
            this.persistentStore.Verify(p => p.Remove(), Times.Never);
            Assert.Equal(1, messages.Count);
        }

        [Fact]
        public void whenCallingOnNetworkConnectedShouldCallSetOnWaitHandle()
        {
            this.service.OnNetworkConnected(this, new EventArgs());
            this.waitHandle.Verify(w => w.Set(), Times.Exactly(1));
        }

        [Fact]
        public void whenCallingOnNetworkConnectedAndServiceIsTerminatingShouldNotCallSetOnWaitHandle()
        {
            this.service.Terminate = true;
            this.service.OnNetworkConnected(this, new EventArgs());
            this.waitHandle.Verify(w => w.Set(), Times.Never());
        }

        private void RemoveMessage(List<IMessage> messages, IMessage message)
        {
            messages.Remove(message);
        }

        private IMessage GetMessage(List<IMessage> messages)
        {
            if (messages.Count > 0)
            {
                return messages[0];
            }
            else
            {
                return null;
            }
        }
    }
}
