using Windows.Networking.Connectivity;

namespace Reyna
{
    public class ConnectionCost : IConnectionCost
    {
        public bool Roaming
        {
            get
            {
                ConnectionProfile internetConnectionProfile = NetworkInformation.GetInternetConnectionProfile();
                return internetConnectionProfile.GetConnectionCost().Roaming;
            }
        }
    }
}