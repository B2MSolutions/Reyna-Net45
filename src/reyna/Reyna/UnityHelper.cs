﻿using Microsoft.Practices.Unity;
using Reyna.Interfaces;
using Reyna.Power;

namespace Reyna
{
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
            container.RegisterType<IConnectionManager, ConnectionManager>();
            container.RegisterType<IPowerManager, PowerManager>();
            container.RegisterType<IConnectionInfo, ConnectionInfo>();
            container.RegisterType<IBlackoutTime, BlackoutTime>();
            container.RegisterType<IWebRequest, ReynaWebRequest>();
            container.RegisterType<INetworkInterfaceWrapper, NetworkInterfaceWrapper>();
            container.RegisterType<INetworkInterfaceWrapperFactory, NetworkInterfaceWrapperFactory>();
            container.RegisterType<IPowerStatusWrapper, PowerStatusWrapper>();

            container.RegisterInstance<IReynaLogger>(new ReynaLogger());

            return container;
        }
    }
}
