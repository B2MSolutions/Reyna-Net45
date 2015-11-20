namespace Reyna
{
    using Reyna.Extensions;
    using Reyna.Interfaces;
    using System;
    using System.Net;
    using System.Text;

    public class ReynaWebRequest : IWebRequest
    {
        private IReynaLogger Logger { get; set; }

        private HttpWebRequest httpRequest;

        public ReynaWebRequest(IReynaLogger logger)
        {
            Logger = logger;
        }

        public void CreateRequest(Uri uri)
        {
            this.httpRequest = WebRequest.Create(uri) as HttpWebRequest;
        }

        public string Method 
        {
            get
            {
                return this.httpRequest.Method;
            }

            set
            {
                this.httpRequest.Method = value;
            }
        }

        public void AddHeader(string key, string value) 
        {
            this.httpRequest.Headers.Add(key, value);
        }

        public string ContentType 
        {
            get
            {
                return this.httpRequest.ContentType;
            }

            set
            {
                this.httpRequest.ContentType = value;
            }
        }

        public Result Send(string content) 
        {
            HttpStatusCode statusCode = HttpStatusCode.NotFound;

            try
            {
                var contentBytes = Encoding.UTF8.GetBytes(content);
                this.httpRequest.ContentLength = contentBytes.Length;

                using (var stream = this.httpRequest.GetRequestStream())
                {
                    stream.Write(contentBytes, 0, contentBytes.Length);
                }

                using (var response = this.httpRequest.GetResponse() as HttpWebResponse)
                {
                    statusCode = this.GetStatusCode(response);
                }
            }
            catch (WebException webException)
            {
                var response = webException.Response as HttpWebResponse;
                statusCode = this.GetStatusCode(response);
                Logger.Error("ReynaWebRequest.Send Error {0} Status code {1}", webException, statusCode);
            }

            return HttpStatusCodeExtensions.ToResult(statusCode);
        }

        internal HttpStatusCode GetStatusCode(HttpWebResponse response)
        {
            if (response == null)
            {
                return HttpStatusCode.ServiceUnavailable;
            }

            return response.StatusCode;
        }
    }
}
