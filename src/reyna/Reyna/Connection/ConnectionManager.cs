namespace Reyna
{
    using Interfaces;

    public class ConnectionManager : IConnectionManager
    {
        private IPreferences Preferences { get; set; }

        private IPowerManager PowerManager { get; set; }

        private IConnectionInfo ConnectionInfo { get; set; }

        private IBlackoutTime BlackoutTime  { get; set; }

        public ConnectionManager(IPreferences preferences, IPowerManager powerManager, IConnectionInfo connectionInfo, IBlackoutTime blackoutTime)
        {
            Preferences = preferences;
            PowerManager = powerManager;
            ConnectionInfo = connectionInfo;
            BlackoutTime = blackoutTime;
        }

        public Result CanSend
        {
            get
            {
                if (!ConnectionInfo.Connected)
                {
                    return Result.NotConnected;
                }

                if (Preferences.WwanBlackoutRange == null)
                {
                    Preferences.SaveCellularDataAsWwanForBackwardsCompatibility();
                }

                if (Preferences.OnChargeBlackout && PowerManager.IsPowerLineConnected())
                {
                    return Result.Blackout;
                }

                if (Preferences.OffChargeBlackout && !PowerManager.IsPowerLineConnected())
                {
                    return Result.Blackout;
                }

                if (Preferences.RoamingBlackout && ConnectionInfo.Roaming)
                {
                    return Result.Blackout;
                } 

                if (!CanSendNow(BlackoutTime, Preferences.WlanBlackoutRange) && ConnectionInfo.Wifi)
                {
                    return Result.Blackout;
                }

                if (!CanSendNow(BlackoutTime, Preferences.WwanBlackoutRange) && ConnectionInfo.Mobile)
                {
                    return Result.Blackout;
                }

                return Result.Ok;
            }
        }

        private bool CanSendNow(IBlackoutTime blackoutTime, string range)
        {
            return blackoutTime.CanSendAtTime(System.DateTime.Now, range);
        }
    }
}
