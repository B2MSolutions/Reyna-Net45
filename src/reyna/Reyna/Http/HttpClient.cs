using Reyna.Interfaces;
using System;

namespace Reyna
{
    public sealed class HttpClient : IHttpClient
    {
        public IConnectionManager ConnectionManager { get; set; }
        private IWebRequest _webRequest;
        private IReynaLogger _logger;
        private ITime _time;

        public HttpClient(IConnectionManager connectionManager, IWebRequest webRequest, 
            IReynaLogger logger, ITime time)
        {
            this.ConnectionManager = connectionManager;
            _webRequest = webRequest;
            _logger = logger;
            _time = time;
        }

        public Result CanSend()
        {
            return this.ConnectionManager.CanSend;
        }

        public Result Post(IMessage message)
        {
            try
            {
                _logger.Info("Reyna.HttpClient Post id {0} url {1} body length {2}", message.Id,message.Url,message.Body.Length);

                Result result = CanSend();
                if (result != Result.Ok)
                {
                    _logger.Info("Reyna.HttpClient Post cannot send: {0}", result.ToString());
                    return result;
                }

                _webRequest.CreateRequest(message.Url);
                _webRequest.Method = "POST";

                foreach (string key in message.Headers.Keys)
                {
                    var value = message.Headers[key];

                    if (key == "content-type")
                    {
                        _webRequest.ContentType = value;
                        continue;
                    }

                    _webRequest.AddHeader(key, value);
                }

                _webRequest.AddHeader("submitted", _time.GetTimeInMilliseconds().ToString());

                _logger.Info("Reyna.HttpClient Post can send: {0}", result.ToString());

                return _webRequest.Send(message.Body);
            }
            catch (Exception e)
            {
                _logger.Error("Reyna.HttpClient Post {0}",e);
               
                return Result.PermanentError;
            }
        }
    }
}
