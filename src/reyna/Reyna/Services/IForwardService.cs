using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reyna.Interfaces
{
    public interface IForwardService : IDisposable
    {
        void Initialize(IRepository sourceStore, IHttpClient httpClient, INetworkStateService networkState, int temporaryErrorMilliseconds, int sleepMilliseconds);

        void Start();

        void Stop();
    }
}
