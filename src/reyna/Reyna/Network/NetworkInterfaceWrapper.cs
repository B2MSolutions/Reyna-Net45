using System.Linq;
using System.Net.NetworkInformation;

namespace Reyna
{
    public class NetworkInterfaceWrapper : INetworkInterfaceWrapper
    {
        private readonly NetworkInterface _networkInterface;

        public NetworkInterfaceWrapper(NetworkInterface networkInterface = null)
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
            get
            {
                if (_networkInterface == null) return NetworkInterfaceType.Unknown;
                return _networkInterface.NetworkInterfaceType;
            }
        }

        public OperationalStatus OperationalStatus
        {
            get
            {
                if (_networkInterface == null) return OperationalStatus.Unknown;
                return _networkInterface.OperationalStatus;
            }
        }
    }
}
