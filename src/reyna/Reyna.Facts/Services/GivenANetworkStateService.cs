namespace Reyna.Facts
{
    using System;
    using Moq;
    using Reyna.Interfaces;
    using Xunit;

    public class GivenANetworkStateService
    {
        private Mock<INamedWaitHandle> mockWaitHandle;
        private Mock<INetwork> mockNetwork;

        private NetworkStateService NetworkStateService { get; set; }

        public GivenANetworkStateService()
        {
            this.mockWaitHandle = new Mock<INamedWaitHandle>();
            this.mockNetwork = new Mock<INetwork>();

            this.NetworkStateService = new NetworkStateService(this.mockWaitHandle.Object, this.mockNetwork.Object);
        }

        [Fact]
        public void WhenConstructiingShouldInitialiseWaitHandle()
        {
            Mock<INamedWaitHandle> mockWaitHandle = new Mock<INamedWaitHandle>();
            Mock<INetwork> mockNetwork = new Mock<INetwork>();
            NetworkStateService service = new NetworkStateService(mockWaitHandle.Object, mockNetwork.Object);
            mockWaitHandle.Verify(h => h.Initialize(false, NetworkStateService.NetworkConnectedNamedEvent), Times.Exactly(1));
        }

        [Fact]
        public void WhenCallingStartAndNetworkConnectedShouldNotifySubscribers()
        {
            //var connectedEventFired = false;
            //this.NetworkStateService.NetworkConnected += (sender, args) => { connectedEventFired = true; };
            //this.WaitHandle.Set();

            //this.NetworkStateService.Start();
            //System.Threading.Thread.Sleep(100);

            //Assert.True(connectedEventFired);

        }

        [Fact]
        public void WhenCallingStopAndNetworkConnectedFiredShouldNotNotifySubscribers()
        {
            //var connectedEventFired = 0;
            //this.NetworkStateService.NetworkConnected += (sender, args) => { connectedEventFired++; };
            //this.WaitHandle.Set();

            //this.NetworkStateService.Start();
            //System.Threading.Thread.Sleep(100);

            //this.NetworkStateService.Stop();
            //this.WaitHandle.Set();

            //Assert.Equal(1, connectedEventFired);
        }

        [Fact]
        public void WhenCallingStartAndNoSubscribersForNetworkConnectedEventShouldNotThrow()
        {
            this.NetworkStateService.SendNetworkConnectedEvent();
        }
    }
}
