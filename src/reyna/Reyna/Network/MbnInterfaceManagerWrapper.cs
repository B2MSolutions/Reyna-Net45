using System;
using System.Runtime.InteropServices;
using MbnApi;

namespace Reyna
{
    public class MbnInterfaceManagerWrapper : IMbnInterfaceManagerWrapper
    {
        private readonly IReynaLogger _logger;

        private MbnInterfaceManager _mbnInterfaceManager;

        private bool _canInitialiseMbnInterfaceManager;

        public MbnInterfaceManagerWrapper(IReynaLogger logger)
        {
            _logger = logger;
            _mbnInterfaceManager = null;
            _canInitialiseMbnInterfaceManager = true;
        }

        private MbnInterfaceManager MbnInterfaceManager
        {
            get
            {
                if (_mbnInterfaceManager != null)    
                    return _mbnInterfaceManager;

                if (!_canInitialiseMbnInterfaceManager)
                    return null;

                try
                {
                    _mbnInterfaceManager = new MbnInterfaceManager();

                    var availableInterfaces = ((IMbnInterfaceManager)_mbnInterfaceManager).GetInterfaces();
                    if (availableInterfaces != null)
                    {
                        return _mbnInterfaceManager;
                    }
                }
                catch (Exception e)
                {
                    var exception = e as COMException;
                    if (exception != null && exception.ErrorCode == -2147221164)
                    {
                        _logger.Verbose("MbnInterfaceManagerWrapper.MbnInterfaceManagerWrapper - No mobile data feature available");
                    }
                    else
                    {
                        _logger.Error("MbnInterfaceManagerWrapper.MbnInterfaceManagerWrapper {0}", e.Message);
                    }
                }

                _canInitialiseMbnInterfaceManager = false;
                _mbnInterfaceManager = null;
                return null;
            }
        }

        public IMbnInterface[] MobileInterfaces
        {
            get
            {
                if (MbnInterfaceManager == null) return new IMbnInterface[0];

                return ((IMbnInterfaceManager)MbnInterfaceManager).GetInterfaces();
            }
        }
    }
}
