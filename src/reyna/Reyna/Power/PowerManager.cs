namespace Reyna.Power
{
    using Reyna.Interfaces;

    public class PowerManager : IPowerManager
    {
        public const byte Online = 0x01;

        public SystemPowerStatus SystemPowerStatus
        {
            get
            {
                var systemPowerStatus = new SystemPowerStatus();
                if (NativeMethods.GetSystemPowerStatusEx(systemPowerStatus, 0) == 1)
                {
                    return systemPowerStatus;
                }

                return null;
            }
        }

        public bool IsBatteryCharging()
        {
            var systemPowerStatus = this.SystemPowerStatus;
            if (systemPowerStatus != null)
            {
                if (systemPowerStatus.ACLineStatus.Equals(PowerManager.Online))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
