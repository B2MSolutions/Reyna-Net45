
namespace Reyna.Interfaces
{
    using System;
    using System.Net.NetworkInformation;

    public interface INetwork
    {
        event EventHandler<NetworkAvailabilityEventArgs> StateChanged;
    }
}
