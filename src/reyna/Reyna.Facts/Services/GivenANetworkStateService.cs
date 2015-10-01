namespace Reyna.Facts
{
    using System;
    using Moq;
    using Reyna.Interfaces;
    using Xunit;

    public class GivenANetworkStateService
    {
        public GivenANetworkStateService()
        {

            this.WaitHandle = new AutoResetEventAdapter(false);

            this.NetworkStateService = new NetworkStateService(this.WaitHandle);
        }


        private IWaitHandle WaitHandle { get; set; }

        private NetworkStateService NetworkStateService { get; set; }

        [Fact]
        public void WhenConstructiingShouldNotThrow()
        {
            Assert.NotNull(this.NetworkStateService);
        }

        [Fact]
        public void WhenConstructingWithAllNullParametersShouldThrow()
        {
            var exception = Assert.Throws<ArgumentNullException>(() => new NetworkStateService(null));
            Assert.Equal("waitHandle", exception.ParamName);
        }

        [Fact]
        public void WhenConstructingAndWaitHandleIsNullParametersShouldThrow()
        {
            var exception = Assert.Throws<ArgumentNullException>(() => new NetworkStateService(null));
            Assert.Equal("waitHandle", exception.ParamName);
        }



        [Fact]
        public void WhenCallingStartAndNetworkConnectedShouldNotifySubscribers()
        {
            var connectedEventFired = false;
            this.NetworkStateService.NetworkConnected += (sender, args) => { connectedEventFired = true; };
            this.WaitHandle.Set();

            this.NetworkStateService.Start();
            System.Threading.Thread.Sleep(100);

            Assert.True(connectedEventFired);

        }

        [Fact]
        public void WhenCallingStopAndNetworkConnectedFiredShouldNotNotifySubscribers()
        {
            var connectedEventFired = 0;
            this.NetworkStateService.NetworkConnected += (sender, args) => { connectedEventFired++; };
            this.WaitHandle.Set();

            this.NetworkStateService.Start();
            System.Threading.Thread.Sleep(100);

            this.NetworkStateService.Stop();
            this.WaitHandle.Set();

            Assert.Equal(1, connectedEventFired);
        }

        [Fact]
        public void WhenCallingStartAndNoSubscribersForNetworkConnectedEventShouldNotThrow()
        {
            this.NetworkStateService.SendNetworkConnectedEvent();
        }
    }
}
