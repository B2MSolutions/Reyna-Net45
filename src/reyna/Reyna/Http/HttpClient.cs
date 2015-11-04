namespace Reyna
{
    using System;
    using System.Net;
    using System.Runtime.InteropServices;
    using System.Text;    
    using Extensions;
    using Reyna.Interfaces;
    using Reyna.Power;

    public sealed class HttpClient : IHttpClient
    {
        public IConnectionManager ConnectionManager { get; set; }
        private IWebRequest webRequest;

        public HttpClient(IConnectionManager connectionManager, IWebRequest webRequest)
        {
            this.ConnectionManager = connectionManager;
            this.webRequest = webRequest;
        }

        public Result CanSend()
        {
            return this.ConnectionManager.CanSend;
        }

        public Result Post(IMessage message)
        {
            try
            {
                Result result = CanSend();
                if (result != Result.Ok)
                {
                    return result;
                }

                this.webRequest.CreateRequest(message.Url);
                this.webRequest.Method = "POST";

                foreach (string key in message.Headers.Keys)
                {
                    var value = message.Headers[key];

                    if (key == "content-type")
                    {
                        this.webRequest.ContentType = value;
                        continue;
                    }

                    this.webRequest.AddHeader(key, value);
                }

                return this.webRequest.Send(message.Body);
            }
            catch (Exception)
            {
                return Result.PermanentError;
            }
        }
    }
}
