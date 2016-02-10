
namespace Reyna.Facts
{
    using Microsoft.Practices.Unity;
    using Reyna.Interfaces;
    using Reyna.Power;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Xunit;
    using Reyna;

    public class GivenAUnityHelper
    {
        [Fact]
        public void WhenCallingGetContainerShouldReturnAUnityContainer()
        {
            var container = UnityHelper.GetContainer();
            Assert.IsType(typeof(UnityContainer), container);
        }

        [Fact]
        public void WhenCallingGetConatinerReturnedContainerShouldHaveRequiredTypesRegistered()
        {
            var container = UnityHelper.GetContainer();
            Assert.NotNull(container.Registrations.FirstOrDefault(r => r.RegisteredType == typeof(IHttpClient) && r.MappedToType == typeof(HttpClient)));
            Assert.NotNull(container.Registrations.FirstOrDefault(r => r.RegisteredType == typeof(IPreferences) && r.MappedToType == typeof(Preferences)));
            Assert.NotNull(container.Registrations.FirstOrDefault(r => r.RegisteredType == typeof(IRepository) && r.Name == Constants.Injection.SQLITE_STORE && r.MappedToType == typeof(SQLiteRepository)));
            Assert.NotNull(container.Registrations.FirstOrDefault(r => r.RegisteredType == typeof(IRepository) && r.Name == Constants.Injection.VOLATILE_STORE && r.MappedToType == typeof(InMemoryQueue)));
            Assert.NotNull(container.Registrations.FirstOrDefault(r => r.RegisteredType == typeof(INetworkStateService) && r.MappedToType == typeof(NetworkStateService)));
            Assert.NotNull(container.Registrations.FirstOrDefault(r => r.RegisteredType == typeof(IAutoResetEventAdapter) && r.MappedToType == typeof(AutoResetEventAdapter)));
            Assert.NotNull(container.Registrations.FirstOrDefault(r => r.RegisteredType == typeof(INamedWaitHandle) && r.MappedToType == typeof(NamedWaitHandle)));
            Assert.NotNull(container.Registrations.FirstOrDefault(r => r.RegisteredType == typeof(IStoreService) && r.MappedToType == typeof(StoreService)));
            Assert.NotNull(container.Registrations.FirstOrDefault(r => r.RegisteredType == typeof(IForwardService) && r.MappedToType == typeof(ForwardService)));
            Assert.NotNull(container.Registrations.FirstOrDefault(r => r.RegisteredType == typeof(IEncryptionChecker) && r.MappedToType == typeof(EncryptionChecker)));
            Assert.NotNull(container.Registrations.FirstOrDefault(r => r.RegisteredType == typeof(INetwork) && r.MappedToType == typeof(Network)));
            Assert.NotNull(container.Registrations.FirstOrDefault(r => r.RegisteredType == typeof(IConnectionManager) && r.MappedToType == typeof(ConnectionManager)));
            Assert.NotNull(container.Registrations.FirstOrDefault(r => r.RegisteredType == typeof(IPowerManager) && r.MappedToType == typeof(PowerManager)));
            Assert.NotNull(container.Registrations.FirstOrDefault(r => r.RegisteredType == typeof(IConnectionInfo) && r.MappedToType == typeof(ConnectionInfo)));
            Assert.NotNull(container.Registrations.FirstOrDefault(r => r.RegisteredType == typeof(IBlackoutTime) && r.MappedToType == typeof(BlackoutTime)));
            Assert.NotNull(container.Registrations.FirstOrDefault(r => r.RegisteredType == typeof(IWebRequest) && r.MappedToType == typeof(ReynaWebRequest)));
            Assert.NotNull(container.Registrations.FirstOrDefault(r => r.RegisteredType == typeof(IReynaLogger) && r.MappedToType == typeof(IReynaLogger)));
            Assert.NotNull(container.Registrations.FirstOrDefault(r => r.RegisteredType == typeof(IRegistry) && r.MappedToType == typeof(Registry)));
            Assert.NotNull(container.Registrations.FirstOrDefault(r => r.RegisteredType == typeof(IPowerStatusWrapper) && r.MappedToType == typeof(PowerStatusWrapper)));
            Assert.NotNull(container.Registrations.FirstOrDefault(r => r.RegisteredType == typeof(INetworkInterfaceWrapper) && r.MappedToType == typeof(NetworkInterfaceWrapper)));
            Assert.NotNull(container.Registrations.FirstOrDefault(r => r.RegisteredType == typeof(INetworkInterfaceWrapperFactory) && r.MappedToType == typeof(NetworkInterfaceWrapperFactory)));
            Assert.NotNull(container.Registrations.FirstOrDefault(r => r.RegisteredType == typeof(IConnectionCost) && r.MappedToType == typeof(ConnectionCost)));

            Assert.Equal(23, container.Registrations.Count()); // Always 1 ahead due to the default lifetime manager registration
        }
    }
}
