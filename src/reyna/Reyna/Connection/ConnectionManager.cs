namespace Reyna
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Reyna.Interfaces;
    using Reyna.Power;

    public class ConnectionManager : IConnectionManager
    {
        internal IPreferences Preferences { get; set; }

        internal IPowerManager PowerManager { get; set; }

        internal IConnectionInfo ConnectionInfo { get; set; }

        internal IBlackoutTime BlackoutTime  { get; set; }

        public ConnectionManager(IPreferences preferences, IPowerManager powerManager, IConnectionInfo connectionInfo, IBlackoutTime blackoutTime)
        {
            this.Preferences = preferences;
            this.PowerManager = powerManager;
            this.ConnectionInfo = connectionInfo;
            this.BlackoutTime = blackoutTime;
        }

        public Result CanSend
        {
            get
            {
                if (!this.ConnectionInfo.Connected)
                {
                    return Result.NotConnected;
                }

                if (Preferences.WwanBlackoutRange == null)
                {
                    Preferences.SaveCellularDataAsWwanForBackwardsCompatibility();
                }

                if (this.Preferences.OnChargeBlackout && this.PowerManager.IsPowerLineConnected())
                {
                    return Result.Blackout;
                }

                if (this.Preferences.OffChargeBlackout && !this.PowerManager.IsPowerLineConnected())
                {
                    return Result.Blackout;
                }

                if (this.Preferences.RoamingBlackout && this.ConnectionInfo.Roaming)
                {
                    return Result.Blackout;
                } 

                if (!this.CanSendNow(this.BlackoutTime, this.Preferences.WlanBlackoutRange) && this.ConnectionInfo.Wifi)
                {
                    return Result.Blackout;
                }

                if (!this.CanSendNow(this.BlackoutTime, this.Preferences.WwanBlackoutRange) && this.ConnectionInfo.Mobile)
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
