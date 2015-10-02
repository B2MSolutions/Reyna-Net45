
namespace Reyna
{
    using System;
    using Reyna.Interfaces;
    using System.Net.NetworkInformation;

    public class Network : INetwork
    {

        public event EventHandler<NetworkAvailabilityEventArgs> StateChanged;

        public Network()
        {
            NetworkChange.NetworkAvailabilityChanged += NetworkAvailabilityChanged;
        }

        private void NetworkAvailabilityChanged(object sender, NetworkAvailabilityEventArgs e)
        {
            if (StateChanged != null)
            {
                this.StateChanged.Invoke(this, e);
            }
        }
    }
}
