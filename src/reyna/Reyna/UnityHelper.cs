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
            container.RegisterType<INamedWaitHandle, NamedWaitHandle>();
            container.RegisterType<IAutoResetEventAdapter, AutoResetEventAdapter>();
            container.RegisterType<IStoreService, StoreService>();
            container.RegisterType<IForwardService, ForwardService>();
            container.RegisterType<IEncryptionChecker, EncryptionChecker>();
            container.RegisterType<INetwork, Network>();

            return container;
        }
    }
}
