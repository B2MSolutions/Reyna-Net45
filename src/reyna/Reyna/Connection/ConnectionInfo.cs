namespace Reyna
{
    using System;
    using System.IO;
    using Microsoft.Win32;
    using System.Net.NetworkInformation;

    public class ConnectionInfo : IConnectionInfo
    {
        public ConnectionInfo()
        {
            this.Registry = new Registry();
        }
        
        public bool Connected
        {
            get
            {
                try
                {
                    foreach (var ni in NetworkInterface.GetAllNetworkInterfaces())
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
                    var interfaces = NetworkInterface.GetAllNetworkInterfaces();
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
                    var interfaces = NetworkInterface.GetAllNetworkInterfaces();
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
                var phoneRoamingBitMask = 0x200;
                var status = this.Registry.GetDWord(Microsoft.Win32.Registry.LocalMachine, "System\\State\\Phone", "Status", 0);
                return (status & phoneRoamingBitMask) == phoneRoamingBitMask;
            }
        }

        internal IRegistry Registry { get; set; }

 
        private bool GPRSNetwork(NetworkInterface ni)
        {
            return ni.NetworkInterfaceType == NetworkInterfaceType.Wwanpp;
        }

        private bool WifiNetwork(NetworkInterface ni)
        {
            return ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211;
        }
    }
}
