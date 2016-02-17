using System;

namespace Reyna.Interfaces
{
    public interface IForwardService : IDisposable
    {
        void Initialize(IRepository sourceStore, IHttpClient httpClient, INetworkStateService networkState, int temporaryErrorMilliseconds, int sleepMilliseconds, bool batchUpload);

        void Start();

        void Stop();
    }
}
