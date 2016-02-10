namespace Reyna
{
    using System;
    using System.Net.NetworkInformation;

    public class ConnectionInfo : IConnectionInfo
    {
        private readonly IConnectionCost _connectionCost;
        private readonly INetworkInterfaceWrapperFactory _networkInterfaceWrapper;

        public ConnectionInfo(IConnectionCost connectionCost, INetworkInterfaceWrapperFactory networkInterfaceWrapperFactory)
        {
            _connectionCost = connectionCost;
            _networkInterfaceWrapper = networkInterfaceWrapperFactory;
        }

        public bool Connected
        {
            get
            {
                try
                {
                    foreach (var ni in _networkInterfaceWrapper.GetAllNetworkInterfaces())
                    {
                        if (ni.OperationalStatus == OperationalStatus.Up && ni.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                        {
                            return true;
                        }
                    }
                }
                catch (Exception)
                { 
                }
                
                return false;
            }
        }

        public bool Mobile
        {
            get
            {
                try
                {
                    var interfaces = _networkInterfaceWrapper.GetAllNetworkInterfaces();
                    bool mobileNetworkConnected = false;
                    bool otherNetworksConnected = false;
                    foreach (var ni in interfaces)
                    {
                        if (GPRSNetwork(ni))
                        {
                            if (ni.OperationalStatus == OperationalStatus.Up)
                            {
                                mobileNetworkConnected = true;
                            }
                        }
                        else if (ni.NetworkInterfaceType != NetworkInterfaceType.Loopback && ni.OperationalStatus == OperationalStatus.Up)
                        {
                            otherNetworksConnected = true;
                        }
                    }

                    return mobileNetworkConnected && !otherNetworksConnected;
                }
                catch (Exception)
                {
                    return false;
                }               
            }
        }

        public bool Wifi
        {
            get
            {
                try
                {
                    var interfaces = _networkInterfaceWrapper.GetAllNetworkInterfaces();
                    foreach (var ni in interfaces)
                    {
                        if (WifiNetwork(ni))
                        {
                            if (ni.OperationalStatus == OperationalStatus.Up)
                            {
                                return true;
                            }
                        }
                    }

                    return false;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        public bool Roaming
        {
            get
            {
                try
                {
                    return _connectionCost.Roaming;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }
 
        private bool GPRSNetwork(INetworkInterfaceWrapper ni)
        {
            return ni.NetworkInterfaceType.Equals(NetworkInterfaceType.Wman) ||
                   ni.NetworkInterfaceType.Equals(NetworkInterfaceType.Wwanpp) ||
                   ni.NetworkInterfaceType.Equals(NetworkInterfaceType.Wwanpp2);
        }

        private bool WifiNetwork(INetworkInterfaceWrapper ni)
        {
            return ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211;
        }
    }
}
