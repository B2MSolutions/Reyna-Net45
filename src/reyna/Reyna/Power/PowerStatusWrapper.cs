using System.Windows.Forms;

namespace Reyna.Power
{
    public class PowerStatusWrapper : IPowerStatusWrapper
    {
        public PowerLineStatus PowerLineStatus
        {
            get { return SystemInformation.PowerStatus.PowerLineStatus; }
        }
    }
}