using System.Net.NetworkInformation;

namespace Reyna
{
    public interface INetworkInterfaceWrapper
    {
        NetworkInterfaceType NetworkInterfaceType { get; }
        OperationalStatus OperationalStatus { get; }
    }
}