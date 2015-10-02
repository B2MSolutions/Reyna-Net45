using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reyna.Interfaces
{
    public interface INamedWaitHandle : IWaitHandle        
    {
        void Initialize(bool initialState, string name);
    }
}
