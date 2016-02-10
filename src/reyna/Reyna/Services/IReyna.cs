
namespace Reyna.Interfaces
{
    public interface IReyna : IService
    {
        void Put(IMessage message);
        void EnableLogging(ILogDelegate llog);
        void SetWlanBlackoutRange(string ranges);
        void SetWwanBlackoutRange(string ranges);
        void SetRoamingBlackout(bool blackout);
        void SetOnChargeBlackout(bool blackout);
        void SetOffChargeBlackout(bool blackout);
        void SetStorageSizeLimit(long limit);
        void ResetStorageSizeLimit();
        long StorageSizeLimit { get; }
    }
}
