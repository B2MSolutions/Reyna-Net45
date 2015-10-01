namespace Reyna
{
    using Microsoft.Practices.Unity;
    using Reyna.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    
    internal static class UnityHelper
    {
        internal static IUnityContainer GetContainer()
        {
            var container = new UnityContainer();

            container.RegisterType<IHttpClient, HttpClient>();
            container.RegisterType<IPreferences, Preferences>();
            container.RegisterType<IRepository, SQLiteRepository>(Constants.Injection.SQLITE_STORE);
            container.RegisterType<IRepository, InMemoryQueue>(Constants.Injection.VOLATILE_STORE);
            container.RegisterType<INetworkStateService, NetworkStateService>();
            container.RegisterType<IWaitHandle, NamedWaitHandle>(Constants.Injection.NETWORK_WAIT_HANDLE);
            container.RegisterType<IWaitHandle, AutoResetEventAdapter>(Constants.Injection.FORWARD_WAIT_HANDLE);
            container.RegisterType<IWaitHandle, AutoResetEventAdapter>(Constants.Injection.STORE_WAIT_HANDLE);
            container.RegisterType<IService, StoreService>(Constants.Injection.STORE_SERVICE);
            container.RegisterType<IService, ForwardService>(Constants.Injection.FORWARD_SERVICE);
            container.RegisterType<IEncryptionChecker, EncryptionChecker>();

            return container;
        }
    }
}
