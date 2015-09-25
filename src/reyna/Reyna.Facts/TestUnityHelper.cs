namespace Reyna.Facts
{
    using Microsoft.Practices.Unity;
    using Reyna.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Moq;
    using Reyna;

    public class TestUnityHelper
    {
        public Mock<IHttpClient> mockHttpClient = new Mock<IHttpClient>();
        public Mock<IPreferences> mockPreferences = new Mock<IPreferences>();
        public Mock<IRepository> mockVolatileStore = new Mock<IRepository>();
        public Mock<IRepository> mockSqlStore = new Mock<IRepository>();
        public Mock<INetworkStateService> mockNetworkStateService = new Mock<INetworkStateService>();
        public Mock<IWaitHandle> mockNetworkWaitHandle = new Mock<IWaitHandle>();
        public Mock<ISystemNotifier> mockSystemNotifier = new Mock<ISystemNotifier>();
        public Mock<IWaitHandle> mockForwardWaitHandle = new Mock<IWaitHandle>();
        public Mock<IWaitHandle> mockStoreWaitHandle = new Mock<IWaitHandle>();
        public Mock<IService> mockStoreService = new Mock<IService>();
        public Mock<IService> mockForwardService = new Mock<IService>();

        public IUnityContainer GetContainer()
        {
            var container = new UnityContainer();

            container.RegisterInstance<IHttpClient>(this.mockHttpClient.Object);
            container.RegisterInstance<IPreferences>(this.mockPreferences.Object);
            container.RegisterInstance<IRepository>(Constants.Injection.VOLATILE_STORE, this.mockVolatileStore.Object);
            container.RegisterInstance <IRepository>(Constants.Injection.SQLITE_STORE, this.mockSqlStore.Object);
            container.RegisterInstance<INetworkStateService>(this.mockNetworkStateService.Object);
            container.RegisterInstance<IWaitHandle>(Constants.Injection.NETWORK_WAIT_HANDLE, this.mockNetworkWaitHandle.Object);
            container.RegisterInstance<ISystemNotifier>(this.mockSystemNotifier.Object);
            container.RegisterInstance<IWaitHandle>(Constants.Injection.FORWARD_WAIT_HANDLE, this.mockForwardWaitHandle.Object);
            container.RegisterInstance<IWaitHandle>(Constants.Injection.STORE_WAIT_HANDLE, this.mockStoreWaitHandle.Object);
            container.RegisterInstance<IService>(Constants.Injection.STORE_SERVICE, this.mockStoreService.Object);
            container.RegisterInstance<IService>(Constants.Injection.FORWARD_SERVICE, this.mockForwardService.Object);

            return container;
        }
    }
}