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
        //private Mock<IHttpClient> HttpClient { get; set; }

        //private Mock<INetworkStateService> NetworkStateService { get; set; }

        //private IAutoResetEventAdapter WaitHandle { get; set; }

        //public GivenAForwardService()
        //{
        //    this.HttpClient = new Mock<IHttpClient>();
        //    this.NetworkStateService = new Mock<INetworkStateService>();
        //    this.WaitHandle = new AutoResetEventAdapter();

        //    this.HttpClient.Setup(c => c.Post(It.IsAny<IMessage>()))
        //        .Returns(Result.Ok);
        //}

        //[Fact]
        //public void WhenCallingStartAndMessageAddedShouldCallPostOnHttpClient()
        //{
        //    var repo = new InMemoryQueue();
        //    ForwardService service = new ForwardService(repo, this.HttpClient.Object, this.NetworkStateService.Object, this.WaitHandle, 100, 0);
        //    var message = this.CreateMessage();
        //    service.Start();

        //    repo.Add(message);
        //    Thread.Sleep(200);

        //    service.Stop();
        //    Assert.Null(repo.Get());
        //    this.HttpClient.Verify(c => c.Post(It.IsAny<IMessage>()), Times.Once());
        //}

        //[Fact]
        //public void WhenCallingStartAndMessageAddedThenImmediatelyStopShouldNotCallPostOnHttpClient()
        //{
        //    var store = new InMemoryQueue();
        //    var service = new ForwardService(store, this.HttpClient.Object, this.NetworkStateService.Object, this.WaitHandle, 100, 0);

        //    var message = this.CreateMessage();

        //    service.Start();
        //    Thread.Sleep(50);

        //    store.Add(message);
        //    service.Stop();
        //    Thread.Sleep(200);

        //    store.Add(this.CreateMessage());
        //    Thread.Sleep(200);

        //    Assert.NotNull(store.Get());
        //    this.HttpClient.Verify(c => c.Post(It.IsAny<IMessage>()), Times.AtMostOnce());
        //}

        //[Fact]
        //public void WhenCallingStartThenStopThenStartShouldPostAllMessages()
        //{
        //    var store = new InMemoryQueue();
        //    var service = new ForwardService(store, this.HttpClient.Object, this.NetworkStateService.Object, this.WaitHandle, 100, 0);

        //    var message = this.CreateMessage();

        //    service.Start();
        //    Thread.Sleep(50);

        //    store.Add(message);
        //    service.Stop();
        //    Thread.Sleep(200);

        //    store.Add(this.CreateMessage());
        //    store.Add(this.CreateMessage());

        //    service.Start();
        //    Thread.Sleep(200);

        //    Assert.Null(store.Get());
        //    this.HttpClient.Verify(c => c.Post(It.IsAny<IMessage>()), Times.Exactly(3));
        //}

        //[Fact]
        //public void WhenCallingStartAndStopRapidlyWhilstAddingMessagesShouldPostAllMessages()
        //{
        //    var store = new InMemoryQueue();
        //    var service = new ForwardService(store, this.HttpClient.Object, this.NetworkStateService.Object, this.WaitHandle, 100, 0);

        //    var messageAddingThread = new Thread(new ThreadStart(() =>
        //    {
        //        for (int j = 0; j < 10; j++)
        //        {
        //            store.Add(new Message(new Uri("http://www.google.com"), string.Empty));
        //            Thread.Sleep(100);
        //        }
        //    }));

        //    messageAddingThread.Start();
        //    Thread.Sleep(50);

        //    for (int k = 0; k < 10; k++)
        //    {
        //        service.Start();
        //        Thread.Sleep(50);

        //        service.Stop();
        //        Thread.Sleep(200);
        //    }

        //    Thread.Sleep(1000);

        //    Assert.Null(store.Get());
        //    this.HttpClient.Verify(c => c.Post(It.IsAny<IMessage>()), Times.Exactly(10));
        //}

        //// This test has dubious value as it is completely reliant on the timing of the thread sleep and 
        //// is likely to produce random failures.
        //[Fact(Skip="true")]
        //public void WhenCallingStopShouldExitImmediately()
        //{
        //    var store = new InMemoryQueue();
        //    var service = new ForwardService(store, this.HttpClient.Object, this.NetworkStateService.Object, this.WaitHandle, 100, 0);

        //    for (int j = 0; j < 200; j++)
        //    {
        //        store.Add(new Message(new Uri("http://www.google.com"), string.Empty));
        //    }

        //    service.Start();

        //    service.Stop();

        //    Thread.Sleep(1000);

        //    Assert.NotNull(store.Get());
        //}

        //[Fact]
        //public void WhenCallingStopOnForwareServiceThatHasntStartedShouldNotThrow()
        //{
        //    var store = new InMemoryQueue();
        //    var service = new ForwardService(store, this.HttpClient.Object, this.NetworkStateService.Object, this.WaitHandle, 100, 0);

        //    service.Stop();
        //}

        //[Fact]
        //public void WhenCallingDisposeShouldNotThrow()
        //{
        //    var store = new InMemoryQueue();
        //    var service = new ForwardService(store, this.HttpClient.Object, this.NetworkStateService.Object, this.WaitHandle, 100, 0);

        //    service.Dispose();
        //}

        //[Fact]
        //public void WhenConstructingWithAllNullParametersShouldThrow()
        //{
        //    var exception = Assert.Throws<ArgumentNullException>(() => new ForwardService(null, null, null, this.WaitHandle, 0, 0));
        //    Assert.Equal("sourceStore", exception.ParamName);
        //}

        //[Fact]
        //public void WhenConstructingWithNullSourceStoreParameterShouldThrow()
        //{
        //    var exception = Assert.Throws<ArgumentNullException>(() => new ForwardService(null, new Mock<IHttpClient>().Object, new Mock<INetworkStateService>().Object, this.WaitHandle, 0, 0));
        //    Assert.Equal("sourceStore", exception.ParamName);
        //}

        //[Fact]
        //public void WhenConstructingWithNullHttpClientParameterShouldThrow()
        //{
        //    var exception = Assert.Throws<ArgumentNullException>(() => new ForwardService(new Mock<IRepository>().Object, null, new Mock<INetworkStateService>().Object, this.WaitHandle, 0, 0));
        //    Assert.Equal("httpClient", exception.ParamName);
        //}

        //[Fact]
        //public void WhenConstructingWithNullNetworkStateParameterShouldThrow()
        //{
        //    var exception = Assert.Throws<ArgumentNullException>(() => new ForwardService(new Mock<IRepository>().Object, new Mock<IHttpClient>().Object, null, this.WaitHandle, 0, 0));
        //    Assert.Equal("networkState", exception.ParamName);
        //}

        //[Fact]
        //public void WhenConstructingWithNullWaitHandleStateParameterShouldThrow()
        //{
        //    var exception = Assert.Throws<ArgumentNullException>(() => new ForwardService(new Mock<IRepository>().Object, new Mock<IHttpClient>().Object, new Mock<INetworkStateService>().Object, null, 0, 0));
        //    Assert.Equal("waitHandle", exception.ParamName);
        //}

        //[Fact]
        //public void WhenCallingStartStopDisposeShouldNotThrow()
        //{
        //    var store = new InMemoryQueue();
        //    var service = new ForwardService(store, this.HttpClient.Object, this.NetworkStateService.Object, this.WaitHandle, 100, 0);

        //    service.Start();
        //    Thread.Sleep(50);

        //    service.Stop();
        //    Thread.Sleep(50);

        //    service.Dispose();
        //}

        //[Fact]
        //public void WhenPostingMessagesAndPermanentErrorShouldRemoveMessageFromQueue()
        //{
        //    var store = new InMemoryQueue();
        //    var service = new ForwardService(store, this.HttpClient.Object, this.NetworkStateService.Object, this.WaitHandle, 100, 0);

        //    this.HttpClient.Setup(hc => hc.Post(It.IsAny<IMessage>())).Returns(Result.PermanentError);

        //    var message = this.CreateMessage();
        //    service.Start();

        //    store.Add(message);
        //    Thread.Sleep(200);

        //    Assert.Null(store.Get());
        //    this.HttpClient.Verify(c => c.Post(It.IsAny<IMessage>()), Times.Once());
        //}

        //[Fact]
        //public void WhenPostingMessagesAndTemporaryErrorShouldNotRemoveMessageFromQueue()
        //{
        //    var store = new InMemoryQueue();
        //    var service = new ForwardService(store, this.HttpClient.Object, this.NetworkStateService.Object, this.WaitHandle, 100, 0);

        //    this.HttpClient.Setup(hc => hc.Post(It.IsAny<IMessage>())).Returns(Result.TemporaryError);

        //    var message = this.CreateMessage();
        //    service.Start();

        //    store.Add(message);
        //    Thread.Sleep(200);

        //    Assert.NotNull(store.Get());
        //    this.HttpClient.Verify(c => c.Post(It.IsAny<IMessage>()), Times.Once());
        //}

        //[Fact]
        //public void WhenCallingNetworkStateChangeShouldPostAllMessages()
        //{
        //    var store = new InMemoryQueue();
        //    var service = new ForwardService(store, this.HttpClient.Object, this.NetworkStateService.Object, this.WaitHandle, 100, 0);

        //    var networkStateWaitHandle = new AutoResetEventAdapter(false);
        //    var networkState = new NetworkStateService(networkStateWaitHandle);

        //    service = new ForwardService(store, this.HttpClient.Object, networkState, this.WaitHandle, 100, 0);

        //    var returnResult = Result.TemporaryError;
        //    this.HttpClient.Setup(c => c.Post(It.IsAny<IMessage>()))
        //        .Returns(() => returnResult);

        //    var message = this.CreateMessage();

        //    service.Start();
        //    networkState.Start();
        //    Thread.Sleep(100);

        //    store.Add(message);
        //    store.Add(message);
        //    store.Add(message);
        //    Thread.Sleep(200);

        //    returnResult = Result.Ok;
        //    networkStateWaitHandle.Set();
        //    Thread.Sleep(6000);

        //    Assert.Null(store.Get());
        //}

        //[Fact]
        //public void WhenCallingNetworkStateChangeAndServiceStopedShouldNotPostAllMessages()
        //{

        //    var networkStateWaitHandle = new AutoResetEventAdapter(false);
        //    var networkState = new NetworkStateService(networkStateWaitHandle);

        //    var store = new InMemoryQueue();
        //    var service = new ForwardService(store, this.HttpClient.Object, networkState, networkStateWaitHandle, 100, 0);

        //    var returnResult = Result.TemporaryError;
        //    this.HttpClient.Setup(c => c.Post(It.IsAny<IMessage>()))
        //        .Returns(() => returnResult);

        //    var message = this.CreateMessage();

        //    service.Start();
        //    networkState.Start();
        //    Thread.Sleep(50);

        //    store.Add(message);
        //    Thread.Sleep(200);

        //    service.Stop();
        //    returnResult = Result.Ok;
        //    networkStateWaitHandle.Set();
        //    Thread.Sleep(500);

        //    Assert.NotNull(store.Get());
        //}

        //[Fact]
        //public void WhenReceivingTemporaryErrorMessageFromServerShouldSleepFor5Minutes()
        //{
        //    this.HttpClient.Setup(c => c.Post(It.IsAny<IMessage>()))
        //        .Returns(Result.TemporaryError);

        //    var waitHandle = new Mock<IWaitHandle>();
        //    var store = new Mock<IRepository>();
        //    store.Setup(s => s.Get()).Returns(this.CreateMessage());

        //    var forwardService = new ForwardService(store.Object, this.HttpClient.Object, this.NetworkStateService.Object, waitHandle.Object, 1000, 0);
        //    forwardService.Start();
        //    Thread.Sleep(500);
        //    forwardService.Stop();

        //    store.Verify(s => s.Get(), Times.Once());
        //}

        //[Fact]
        //public void WhenSendingMessagesShouldSleepFor1SecondBetweenEachMessage()
        //{
        //    this.HttpClient.Setup(c => c.Post(It.IsAny<IMessage>()))
        //        .Returns(Result.Ok);

        //    var waitHandle = new Mock<IWaitHandle>();
        //    var store = new Mock<IRepository>();
        //    store.Setup(s => s.Get()).Returns(this.CreateMessage());

        //    var forwardService = new ForwardService(store.Object, this.HttpClient.Object, this.NetworkStateService.Object, waitHandle.Object, 1000, 1000);
        //    forwardService.Start();
        //    Thread.Sleep(3000);
        //    forwardService.Stop();

        //    store.Verify(s => s.Get(), Times.AtMost(3));
        //}

        //[Fact]
        //public void ShouldSetTemporaryErrorMillisecondsTo5Minutes()
        //{
        //    var store = new InMemoryQueue();

        //    var forwardService = new ForwardService(store, this.HttpClient.Object, this.NetworkStateService.Object, this.WaitHandle, 300000, 0);
        //    Assert.Equal(300000, forwardService.TemporaryErrorMilliseconds);
        //}

        //[Fact]
        //public void ShouldSetSleepMillisecondsTo1Second()
        //{
        //    var store = new InMemoryQueue();

        //    var forwardService = new ForwardService(store, this.HttpClient.Object, this.NetworkStateService.Object, this.WaitHandle, 300000, 1000);
        //    Assert.Equal(1000, forwardService.SleepMilliseconds);
        //}

        //[Fact]
        //public void WhenReceivingBlackoutErrorShouldNotSleepAndNotDeletingMessages()
        //{
        //    this.HttpClient.Setup(c => c.Post(It.IsAny<IMessage>()))
        //        .Returns(Result.Blackout);

        //    var waitHandle = new Mock<IWaitHandle>();
        //    var store = new Mock<IRepository>();
        //    store.Setup(s => s.Get()).Returns(this.CreateMessage());

        //    var forwardService = new ForwardService(store.Object, this.HttpClient.Object, this.NetworkStateService.Object, waitHandle.Object, 1000, 0);
        //    forwardService.Start();
        //    Thread.Sleep(500);
        //    forwardService.Stop();

        //    store.Verify(s => s.Get(), Times.Once());
        //    store.Verify(s => s.Remove(), Times.Never());
        //}

        //[Fact]
        //public void WhenReceivingNotConnectedErrorShouldNotSleepAndNotDeletingMessages()
        //{
        //    this.HttpClient.Setup(c => c.Post(It.IsAny<IMessage>()))
        //        .Returns(Result.NotConnected);

        //    var waitHandle = new Mock<IWaitHandle>();
        //    var store = new Mock<IRepository>();
        //    store.Setup(s => s.Get()).Returns(this.CreateMessage());

        //    var forwardService = new ForwardService(store.Object, this.HttpClient.Object, this.NetworkStateService.Object, waitHandle.Object, 1000, 0);
        //    forwardService.Start();
        //    Thread.Sleep(500);
        //    forwardService.Stop();

        //    store.Verify(s => s.Get(), Times.Once());
        //    store.Verify(s => s.Remove(), Times.Never());
        //}

    //    [Fact]
    //    public void WhenReceivingTemporaryErrorMessageFromServerAndSleepFor5MinutesThenTerminateSignaledShouldExit()
    //    {
    //        this.HttpClient.Setup(c => c.Post(It.IsAny<IMessage>()))
    //            .Returns(Result.TemporaryError);

    //        var waitHandle = new Mock<IWaitHandle>();
    //        var store = new Mock<IRepository>();
    //        store.Setup(s => s.Get()).Returns(this.CreateMessage());
    //        var forwardService = new ForwardService(store.Object, this.HttpClient.Object, this.NetworkStateService.Object, waitHandle.Object, 5 * 60 * 1000, 0);
    //        new Thread(this.StopForwardService).Start(forwardService);
    //        forwardService.Start();
    //        Thread.Sleep(1000);
    //        store.Verify(s => s.Get(), Times.Once());
    //    }

    //    private void StopForwardService(object forwardService)
    //    {
    //        Thread.Sleep(100);
    //        ((ForwardService)forwardService).Stop();
    //    }

    //    private IMessage CreateMessage()
    //    {
    //        return new Message(new Uri("http://test.com"), "BODY");
    //    }

    //    private class TestRepository
    //    {
    //        public TestRepository()
    //        {
    //            this.Messages = new Queue<IMessage>();
    //            this.Repository = new Mock<IRepository>();

    //            this.Repository.Setup(r => r.Add(It.IsAny<IMessage>())).Callback<IMessage>((message) =>
    //            {
    //                this.Messages.Enqueue(message);
    //                this.Repository.Raise(r => r.MessageAdded += null, EventArgs.Empty);
    //            });

    //            this.Repository.Setup(r => r.Get()).Returns(this.Messages.Peek);

    //            this.Repository.Setup(r => r.Remove()).Callback(() =>
    //            {
    //                this.Messages.Dequeue();
    //            });
    //        }

    //        public Mock<IRepository> Repository
    //        {
    //            get;
    //            set;
    //        }

    //        public Queue<IMessage> Messages
    //        {
    //            get;
    //            set;
    //        }
    //    }
    }
}
