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
