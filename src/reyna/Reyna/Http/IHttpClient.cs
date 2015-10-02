namespace Reyna.Interfaces
{
    using System.Net;

    public interface IHttpClient
    {
        IConnectionManager ConnectionManager { get; set; }
        Result Post(IMessage message);
        void SetCertificatePolicy(ICertificatePolicy certificatePolicy);
        Result CanSend();
    }
}
