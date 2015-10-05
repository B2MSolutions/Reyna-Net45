using Reyna.Power;
using System;

namespace Reyna.Interfaces
{
    public interface IPowerManager
    {
        bool IsBatteryCharging();
        SystemPowerStatus SystemPowerStatus { get; }
    }
}
