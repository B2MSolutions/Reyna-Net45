
namespace Reyna.Interfaces
{
    public interface IReyna : IService
    {
        void Put(IMessage message);
        void EnableLogging(ILogDelegate llog);
        void SetStorageSizeLimit(long limit);
        void ResetStorageSizeLimit();
        long StorageSizeLimit { get; }
    }
}
