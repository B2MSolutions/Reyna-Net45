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

            return container;
        }
    }
}
