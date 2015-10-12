
namespace Reyna.Interfaces
{
    using System.Net;

    public interface IServicePoint
    {
        void SetCertificatePolicy(ICertificatePolicy certificatePolicy);
    }
}
