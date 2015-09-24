
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
        }
    }
}
