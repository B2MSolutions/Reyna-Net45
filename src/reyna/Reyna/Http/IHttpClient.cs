namespace Reyna.Interfaces
{
    using System.Net;

    public interface IHttpClient
    {
        IConnectionManager ConnectionManager { get; set; }
        Result Post(IMessage message);
        Result CanSend();
    }
}
