
namespace Reyna.Interfaces
{
    using System;

    public interface IBlackoutTime
    {
        bool CanSendAtTime(DateTime now, string ranges);
        TimeRange ParseTime(string range);
    }
}
