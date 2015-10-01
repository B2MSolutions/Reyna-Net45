namespace Reyna
{
    using System;
    using System.Threading;
    using Reyna.Interfaces;
    using System.Net.NetworkInformation;
    
    internal sealed class NetworkStateService : ThreadWorker, INetworkStateService
    {
        public static readonly string NetworkConnectedNamedEvent = "Reyna_NetworkConnected";

        public NetworkStateService(IWaitHandle waitHandle) : base(waitHandle, false)
        {

        }
        
        public event EventHandler<EventArgs> NetworkConnected;

        public override void Start()
        {
            base.Start();
            NetworkChange.NetworkAvailabilityChanged += NetworkAvailabilityChanged;
        }

        public override void Stop()
        {
            base.Stop();
            NetworkChange.NetworkAvailabilityChanged -= NetworkAvailabilityChanged;
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

        //private void SubscribeToNetworkStateChange()
        //{
        //    this.SystemNotifier.NotifyOnNetworkConnect(NetworkStateService.NetworkConnectedNamedEvent);
        //}

        //private void UnSubscribeToNetworkStateChange()
        //{
        //    this.SystemNotifier.ClearNotification(NetworkStateService.NetworkConnectedNamedEvent);
        //}
    }
}
