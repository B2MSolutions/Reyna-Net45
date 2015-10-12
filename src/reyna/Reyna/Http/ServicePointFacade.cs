
namespace Reyna
{
    using Reyna.Interfaces;
    using System.Net;

    public class ServicePointFacade : IServicePoint
    {
        public void SetCertificatePolicy(ICertificatePolicy certificatePolicy)
        {
#pragma warning disable 0618
            ServicePointManager.CertificatePolicy = certificatePolicy;
#pragma warning restore 0618
        }
    }
}
