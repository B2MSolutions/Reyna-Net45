
namespace Reyna.Facts
{
    using Microsoft.Practices.Unity;
    using Reyna.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Xunit;

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
            Assert.NotNull(container.Registrations.FirstOrDefault(r => r.RegisteredType == typeof(IWaitHandle) && r.Name == Constants.Injection.NETWORK_WAIT_HANDLE && r.MappedToType == typeof(NamedWaitHandle)));
            Assert.NotNull(container.Registrations.FirstOrDefault(r => r.RegisteredType == typeof(IWaitHandle) && r.Name == Constants.Injection.FORWARD_WAIT_HANDLE && r.MappedToType == typeof(AutoResetEventAdapter)));
            Assert.NotNull(container.Registrations.FirstOrDefault(r => r.RegisteredType == typeof(IWaitHandle) && r.Name == Constants.Injection.STORE_WAIT_HANDLE && r.MappedToType == typeof(AutoResetEventAdapter)));
            Assert.NotNull(container.Registrations.FirstOrDefault(r => r.RegisteredType == typeof(IService) && r.Name == Constants.Injection.STORE_SERVICE && r.MappedToType == typeof(StoreService)));
            Assert.NotNull(container.Registrations.FirstOrDefault(r => r.RegisteredType == typeof(IService) && r.Name == Constants.Injection.FORWARD_SERVICE && r.MappedToType == typeof(ForwardService)));
            Assert.NotNull(container.Registrations.FirstOrDefault(r => r.RegisteredType == typeof(IEncryptionChecker) && r.MappedToType == typeof(EncryptionChecker)));
        }
    }
}
