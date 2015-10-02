namespace Reyna
{
    using System;
    using System.Threading;
    using Reyna.Interfaces;
    using System.Net.NetworkInformation;

    internal sealed class NetworkStateService : ThreadWorker, INetworkStateService
    {
        public static readonly string NetworkConnectedNamedEvent = "Reyna_NetworkConnected";
        
        public event EventHandler<EventArgs> NetworkConnected;

        internal INetwork Network { get; set; }

        public NetworkStateService(INamedWaitHandle waitHandle, INetwork network) : base(waitHandle, false)
        {
            waitHandle.Initialize(false, Reyna.NetworkStateService.NetworkConnectedNamedEvent);
            this.Network = network;
        }
        
        public override void Start()
        {
            base.Start();
            this.Network.StateChanged += NetworkAvailabilityChanged;
        }

        public override void Stop()
        {
            base.Stop();
            this.Network.StateChanged -= NetworkAvailabilityChanged;
        }

        internal void SendNetworkConnectedEvent()
        {
            if (this.Terminate)
            {
                return;
            }

            if (this.NetworkConnected == null)
            {
                return;
            }

            this.NetworkConnected.Invoke(this, EventArgs.Empty);
        }

        protected override void ThreadStart()
        {
            while (!this.Terminate)
            {
                this.WaitHandle.WaitOne();

                this.SendNetworkConnectedEvent();

                this.WaitHandle.Reset();
            }
        }

        private void NetworkAvailabilityChanged(object sender, NetworkAvailabilityEventArgs e)
        {
            if (e.IsAvailable)
            {
                this.SendNetworkConnectedEvent();
            }
        }
    }
}
