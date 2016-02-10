using System.Windows.Forms;

namespace Reyna.Power
{
    public interface IPowerStatusWrapper
    {
        PowerLineStatus PowerLineStatus { get; }
    }
}