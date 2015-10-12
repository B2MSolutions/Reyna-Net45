

namespace Reyna.Facts.Http
{
    using Moq;
    using Reyna.Interfaces;
    using System;
    using System.Net;
    using Xunit;

    public class GivenAnHttpClient
    {
        private HttpClient httpClient;
        private Mock<IConnectionManager> connectionManager;
        private Mock<IServicePoint> servicePoint;
        private Mock<IWebRequest> webRequest;

        public GivenAnHttpClient()
        {
            this.connectionManager = new Mock<IConnectionManager>();
            this.servicePoint = new Mock<IServicePoint>();
            this.webRequest = new Mock<IWebRequest>();

            this.httpClient = new HttpClient(connectionManager.Object, this.servicePoint.Object, this.webRequest.Object);
        }

        [Fact]
        public void whenCallingSetCertificatePolicyWithACertificateShouldCallServicePoint()
        {
            Mock<ICertificatePolicy> certificatePolicy = new Mock<ICertificatePolicy>();

            this.httpClient.SetCertificatePolicy(certificatePolicy.Object);

            this.servicePoint.Verify(sp => sp.SetCertificatePolicy(certificatePolicy.Object), Times.Exactly(1));
        }

        [Fact]
        public void whenCallingSetCertificatePolicyWithNoCertificateShouldNotCallServicePoint()
        {
            this.httpClient.SetCertificatePolicy(null);
            this.connectionManager.SetupGet(cm => cm.CanSend).Returns(Result.Ok);

            this.servicePoint.Verify(sp => sp.SetCertificatePolicy(It.IsAny<ICertificatePolicy>()), Times.Never);
        }

        [Theory]
        [InlineData(Result.Blackout)]
        [InlineData(Result.NotConnected)]
        [InlineData(Result.Ok)]
        [InlineData(Result.PermanentError)]
        [InlineData(Result.TemporaryError)]
        public void whenCallingCanSendSouldCallConnectionManagerCanSend(Result expectedResult)
        {
            this.connectionManager.SetupGet(cm => cm.CanSend).Returns(expectedResult);
            Result result = this.httpClient.CanSend();
            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void whenCallingPostAndCanSendShouldCreateAWebRequestAndSetMethodToBePost()
        {
            this.connectionManager.SetupGet(c => c.CanSend).Returns(Result.Ok);
            Uri uri = new System.Uri("http://www.google.com");
            var result = this.httpClient.Post(new Message(uri, "Message Body"));
            this.webRequest.Verify(w => w.CreateRequest(uri), Times.Exactly(1));
            this.webRequest.VerifySet(w => w.Method = "POST", Times.Exactly(1));
            Assert.Equal(Result.Ok, result);
        }

        [Theory]
        [InlineData(Result.Blackout)]
        [InlineData(Result.NotConnected)]
        [InlineData(Result.PermanentError)]
        [InlineData(Result.TemporaryError)]
        public void whenCalingPostAndCanSendDoesNotReturnOkayShouldNotCreateWebRequest(Result expectedResult)
        {
            this.connectionManager.SetupGet(c => c.CanSend).Returns(expectedResult);
            Uri uri = new System.Uri("http://www.google.com");
            var result = this.httpClient.Post(new Message(uri, "Message Body"));
            this.webRequest.Verify(w => w.CreateRequest(uri), Times.Never);
            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void whenCallingPostWithHeadersShouldCorrectlySetHeaderAndContentType()
        {
            Uri uri = new System.Uri("http://www.google.com");
            Message message = new Message(uri, "Message Body");

            message.Headers.Add("header1", "value1");
            message.Headers.Add("content-type", "type");
            message.Headers.Add("header2", "value2");

            var result = this.httpClient.Post(message);

            this.webRequest.VerifySet(w => w.ContentType = "type", Times.Exactly(1));
            this.webRequest.Verify(w => w.AddHeader("header1", "value1"), Times.Exactly(1));
            this.webRequest.Verify(w => w.AddHeader("header2", "value2"), Times.Exactly(1));
        }

        [Fact]
        public void whenCallingPostAndAnExceptionIsThrownShouldReturnPermanentError()
        {
            Uri uri = new System.Uri("http://www.google.com");
            Message message = new Message(uri, "Message Body");

            this.webRequest.Setup(w => w.CreateRequest(It.IsAny<Uri>())).Throws(new Exception("It Broke!"));

            var result = this.httpClient.Post(message);

            Assert.Equal(Result.PermanentError, result);
        }

        [Fact]
        public void whenCallingPostAndClientCanSendShouldCallSendOnWebRequestAndReturnTheResult()
        {
            this.connectionManager.SetupGet(c => c.CanSend).Returns(Result.Ok);
            this.webRequest.Setup(w => w.Send(It.IsAny<String>())).Returns(Result.Ok);
            Uri uri = new System.Uri("http://www.google.com");

            var result = this.httpClient.Post(new Message(uri, "Message Body"));
            
            this.webRequest.Verify(w => w.Send("Message Body"), Times.Exactly(1));
            Assert.Equal(Result.Ok, result);
        }
    }
}
