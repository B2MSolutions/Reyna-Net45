using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reyna.Interfaces
{
    public interface IStoreService : IDisposable
    {
        void Initialize(IRepository sourceStore, IRepository targetStore);
        
        void Start();

        void Stop();
    }
}
