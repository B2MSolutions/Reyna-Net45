using System.Windows.Forms;

namespace Reyna.Power
{
    using Interfaces;

    public class PowerManager : IPowerManager
    {
        private readonly IPowerStatusWrapper _powerStatusWrapper;

        public PowerManager(IPowerStatusWrapper powerStatusWrapper = null)
        {
            _powerStatusWrapper = powerStatusWrapper ?? new PowerStatusWrapper();
        }

        public bool IsPowerLineConnected()
        {
            return _powerStatusWrapper.PowerLineStatus.Equals(PowerLineStatus.Online);
        }
    }

    public interface IPowerStatusWrapper
    {
        PowerLineStatus PowerLineStatus { get; }
    }

    public class PowerStatusWrapper : IPowerStatusWrapper
    {
        public PowerLineStatus PowerLineStatus
        {
            get { return SystemInformation.PowerStatus.PowerLineStatus; }
        }
    }
}
