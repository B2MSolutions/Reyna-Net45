
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
        private IReynaLogger Logger;

        public HttpClient(IConnectionManager connectionManager, IWebRequest webRequest, IReynaLogger logger)
        {
            this.ConnectionManager = connectionManager;
            this.webRequest = webRequest;
            Logger = logger;
        }

        public Result CanSend()
        {
            return this.ConnectionManager.CanSend;
        }

        public Result Post(IMessage message)
        {
            try
            {
                Logger.Info("Reyna.HttpClient Post id {0} url {1} body length {2}", message.Id,message.Url,message.Body.Length);

                Result result = CanSend();
                if (result != Result.Ok)
                {
                    Logger.Info("Reyna.HttpClient Post cannot send");
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
            catch (Exception e)
            {
                Logger.Error("Reyna.HttpClient Post {0}",e);
               
                return Result.PermanentError;
            }
        }
    }
}
