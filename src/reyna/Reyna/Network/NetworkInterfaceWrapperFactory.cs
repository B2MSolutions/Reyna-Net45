using System.Linq;
using System.Net.NetworkInformation;

namespace Reyna
{
    public class NetworkInterfaceWrapperFactory : INetworkInterfaceWrapperFactory
    {
        public INetworkInterfaceWrapper[] GetAllNetworkInterfaces()
        {
            var interfaces = NetworkInterface.GetAllNetworkInterfaces();

            return interfaces.Select(networkInterface => new NetworkInterfaceWrapper(networkInterface)).Cast<INetworkInterfaceWrapper>().ToArray();
        }
    }
}