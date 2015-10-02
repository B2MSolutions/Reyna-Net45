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
        internal Mock<IHttpClient> mockHttpClient = new Mock<IHttpClient>();
        internal Mock<IPreferences> mockPreferences = new Mock<IPreferences>();
        internal Mock<IRepository> mockVolatileStore = new Mock<IRepository>();
        internal Mock<IRepository> mockSqlStore = new Mock<IRepository>();
        internal Mock<INetworkStateService> mockNetworkStateService = new Mock<INetworkStateService>();
        internal Mock<IStoreService> mockStoreService = new Mock<IStoreService>();
        internal Mock<IForwardService> mockForwardService = new Mock<IForwardService>();
        internal Mock<IEncryptionChecker> mockEncryptionChecker = new Mock<IEncryptionChecker>();

        public IUnityContainer GetContainer()
        {
            var container = new UnityContainer();

            container.RegisterInstance<IHttpClient>(this.mockHttpClient.Object);
            container.RegisterInstance<IPreferences>(this.mockPreferences.Object);
            container.RegisterInstance<IRepository>(Constants.Injection.VOLATILE_STORE, this.mockVolatileStore.Object);
            container.RegisterInstance<IRepository>(Constants.Injection.SQLITE_STORE, this.mockSqlStore.Object);
            container.RegisterInstance<INetworkStateService>(this.mockNetworkStateService.Object);
            container.RegisterInstance<IStoreService>(this.mockStoreService.Object);
            container.RegisterInstance<IForwardService>(this.mockForwardService.Object);
            container.RegisterInstance<IEncryptionChecker>(this.mockEncryptionChecker.Object);

            return container;
        }
    }
}