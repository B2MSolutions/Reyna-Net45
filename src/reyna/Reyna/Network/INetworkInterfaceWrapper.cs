using System.Net.NetworkInformation;

namespace Reyna
{
    public interface INetworkInterfaceWrapper
    {
        INetworkInterfaceWrapper[] GetAllNetworkInterfaces();
        NetworkInterfaceType NetworkInterfaceType { get; }
        OperationalStatus OperationalStatus { get; }
    }
}