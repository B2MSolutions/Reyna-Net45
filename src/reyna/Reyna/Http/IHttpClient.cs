namespace Reyna.Interfaces
{
    using System.Net;

    public interface IHttpClient
    {
        Result Post(IMessage message);
        void SetCertificatePolicy(ICertificatePolicy certificatePolicy);
    }
}
