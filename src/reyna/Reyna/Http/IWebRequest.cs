
namespace Reyna
{
    using Reyna.Interfaces;
    using System;

    public interface IWebRequest
    {
        void CreateRequest(Uri uri);
        string Method { get; set; }
        void AddHeader(string key, string value);
        string ContentType { get; set; }
        Result Send(string content);
    }
}
