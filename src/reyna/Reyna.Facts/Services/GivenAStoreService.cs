namespace Reyna.Facts
{
    using System;
    using System.Threading;
    using Microsoft.Win32;
    using Moq;
    using Reyna.Interfaces;
    using Xunit;
    using System.Collections.Generic;

    public class GivenAStoreService
    {
        private StoreService service;
        private Mock<IAutoResetEventAdapter> autoResetEventAdapter;
        private Mock<IRepository> volatileStore;
        private Mock<IRepository> persistentStore;
        private Mock<IPreferences> preferences;

        public GivenAStoreService()
        {
            this.autoResetEventAdapter = new Mock<IAutoResetEventAdapter>();
            this.volatileStore = new Mock<IRepository>();
            this.persistentStore = new Mock<IRepository>();
            this.preferences = new Mock<IPreferences>();

            this.service = new StoreService(this.autoResetEventAdapter.Object, this.preferences.Object);
        }

        [Fact]
        public void whenCallingInitialiseShouldSetUpServiceCorrectly()
        {
            this.service.Initialize(this.volatileStore.Object, this.persistentStore.Object);
            Assert.Equal(this.volatileStore.Object, this.service.SourceStore);
            Assert.Equal(this.persistentStore.Object, this.service.TargetStore);
        }

        [Fact]
        public void whenCallingInitialiseShouldInitialiseStores()
        {
            this.service.Initialize(this.volatileStore.Object, this.persistentStore.Object);
            this.persistentStore.Verify(p => p.Initialise(), Times.Exactly(1));
            this.volatileStore.Verify(v => v.Initialise(), Times.Exactly(1));
        }

        [Fact]
        public void whenCallingInitialiseWithNullSourceStoreShouldThrow()
        {
            Assert.Throws<ArgumentNullException>(() => this.service.Initialize(null, this.persistentStore.Object));
        }

        [Fact]
        public void whenCallingInitialiseWithNullTargetStoreShouldThrow()
        {
            Assert.Throws<ArgumentNullException>(() => this.service.Initialize(this.volatileStore.Object, null));
        }

        [Fact]
        public void whenConstructingAndWaitHandleIsNullShouldThrow()
        {
            Assert.Throws<ArgumentNullException>(() => new StoreService(null, this.preferences.Object));
        }

        [Fact]
        public void whenCallingStartShouldCallWaitHandle()
        {
            this.service.Initialize(this.volatileStore.Object, this.persistentStore.Object);
            
            this.service.Start();
            Thread.Sleep(50);
            this.service.Stop();

            this.autoResetEventAdapter.Verify(a => a.WaitOne(), Times.AtLeastOnce());
            this.autoResetEventAdapter.Verify(a => a.Reset(), Times.AtLeastOnce());
        }

        [Fact]
        public void whenCallingStartAndThereIsAMessageInTheSourceStoreShouldAddItToTheTargetStoreAndRemoveItFromTheSource()
        {
            this.service.Initialize(this.volatileStore.Object, this.persistentStore.Object);
            Message message = new Message(new Uri("http://google.com"), "MessageBody");
            List<IMessage> messages = new List<IMessage> { message };
            
            this.volatileStore.Setup(v => v.Get()).Returns(() => this.GetMessage(messages));
            this.volatileStore.Setup(v => v.Remove()).Callback(() => this.RemoveMessage(messages, message));
            this.preferences.SetupGet(p => p.StorageSizeLimit).Returns(-1);

            this.autoResetEventAdapter.Setup(a => a.Reset()).Callback(() => {
                this.service.Stop();
                this.persistentStore.Verify(p => p.Add(message), Times.Exactly(1));
                Assert.Equal(0, messages.Count);
            });

            this.service.Start();
        }

        [Fact]
        public void whenCallingStartAndThereIsAMessageInTheQueueAndAStorageSizeLimitIsSetShouldCallAddIwthStorageSizeLimit()
        {
            long limit = 123456;

            this.service.Initialize(this.volatileStore.Object, this.persistentStore.Object);
            Message message = new Message(new Uri("http://google.com"), "MessageBody");
            List<IMessage> messages = new List<IMessage> { message };

            this.volatileStore.Setup(v => v.Get()).Returns(() => this.GetMessage(messages));
            this.volatileStore.Setup(v => v.Remove()).Callback(() => this.RemoveMessage(messages, message));
            this.preferences.SetupGet(p => p.StorageSizeLimit).Returns(limit);

            this.autoResetEventAdapter.Setup(a => a.Reset()).Callback(() =>
            {
                this.service.Stop();
                this.persistentStore.Verify(p => p.Add(message, limit), Times.Exactly(1));
                this.persistentStore.Verify(p => p.Add(message), Times.Never);
                Assert.Equal(0, messages.Count);
            });

            this.service.Start();
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
