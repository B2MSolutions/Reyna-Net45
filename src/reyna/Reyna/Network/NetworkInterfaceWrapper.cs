using System.Linq;
using System.Net.NetworkInformation;

namespace Reyna
{
    public class NetworkInterfaceWrapper : INetworkInterfaceWrapper
    {
        private readonly NetworkInterface _networkInterface;

        public NetworkInterfaceWrapper(NetworkInterface networkInterface)
        {
            _networkInterface = networkInterface;
        }

        public INetworkInterfaceWrapper[] GetAllNetworkInterfaces()
        {
            var interfaces = NetworkInterface.GetAllNetworkInterfaces();

            return interfaces.Select(networkInterface => new NetworkInterfaceWrapper(networkInterface)).Cast<INetworkInterfaceWrapper>().ToArray();
        }

        public NetworkInterfaceType NetworkInterfaceType
        {
            get { return _networkInterface.NetworkInterfaceType; }
        }

        public OperationalStatus OperationalStatus
        {
            get { return _networkInterface.OperationalStatus; }
        }
    }
}
