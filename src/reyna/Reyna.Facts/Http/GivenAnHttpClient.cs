using Moq;
using Reyna.Interfaces;
using System;
using Xunit;

namespace Reyna.Facts.Http
{
    public class GivenAnHttpClient
    {
        private HttpClient _httpClient;
        private Mock<IConnectionManager> _connectionManagerMock;
        private Mock<IWebRequest> _webRequestMock;
        private readonly Mock<IReynaLogger> _loggerMock;
        private Mock<ITime> _timeMock;

        public GivenAnHttpClient()
        {
            _connectionManagerMock = new Mock<IConnectionManager>();
            _webRequestMock = new Mock<IWebRequest>();
            _loggerMock = new Mock<IReynaLogger>();
            _timeMock = new Mock<ITime>();
            _httpClient = new HttpClient(_connectionManagerMock.Object, _webRequestMock.Object, _loggerMock.Object, _timeMock.Object);
        }

        [Theory]
        [InlineData(Result.Blackout)]
        [InlineData(Result.NotConnected)]
        [InlineData(Result.Ok)]
        [InlineData(Result.PermanentError)]
        [InlineData(Result.TemporaryError)]
        public void whenCallingCanSendSouldCallConnectionManagerCanSend(Result expectedResult)
        {
            _connectionManagerMock.SetupGet(cm => cm.CanSend).Returns(expectedResult);
            Result result = _httpClient.CanSend();
            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void whenCallingPostAndCanSendShouldCreateAWebRequestAndSetMethodToBePost()
        {
            _connectionManagerMock.SetupGet(c => c.CanSend).Returns(Result.Ok);
            Uri uri = new System.Uri("http://www.google.com");
            var result = _httpClient.Post(new Message(uri, "Message Body"));
            _webRequestMock.Verify(w => w.CreateRequest(uri), Times.Exactly(1));
            _webRequestMock.VerifySet(w => w.Method = "POST", Times.Exactly(1));
            Assert.Equal(Result.Ok, result);
        }

        [Theory]
        [InlineData(Result.Blackout)]
        [InlineData(Result.NotConnected)]
        [InlineData(Result.PermanentError)]
        [InlineData(Result.TemporaryError)]
        public void whenCalingPostAndCanSendDoesNotReturnOkayShouldNotCreateWebRequest(Result expectedResult)
        {
            _connectionManagerMock.SetupGet(c => c.CanSend).Returns(expectedResult);
            Uri uri = new System.Uri("http://www.google.com");
            var result = _httpClient.Post(new Message(uri, "Message Body"));
            _webRequestMock.Verify(w => w.CreateRequest(uri), Times.Never);
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

            var result = _httpClient.Post(message);

            _webRequestMock.VerifySet(w => w.ContentType = "type", Times.Exactly(1));
            _webRequestMock.Verify(w => w.AddHeader("header1", "value1"), Times.Exactly(1));
            _webRequestMock.Verify(w => w.AddHeader("header2", "value2"), Times.Exactly(1));
        }

        [Fact]
        public void whenCallingPostAndAnExceptionIsThrownShouldReturnPermanentError()
        {
            Uri uri = new System.Uri("http://www.google.com");
            Message message = new Message(uri, "Message Body");

            _webRequestMock.Setup(w => w.CreateRequest(It.IsAny<Uri>())).Throws(new Exception("It Broke!"));

            var result = _httpClient.Post(message);

            Assert.Equal(Result.PermanentError, result);
        }

        [Fact]
        public void whenCallingPostAndClientCanSendShouldCallSendOnWebRequestAndReturnTheResult()
        {
            _connectionManagerMock.SetupGet(c => c.CanSend).Returns(Result.Ok);
            _webRequestMock.Setup(w => w.Send(It.IsAny<String>())).Returns(Result.Ok);
            Uri uri = new System.Uri("http://www.google.com");

            var result = _httpClient.Post(new Message(uri, "Message Body"));
            
            _webRequestMock.Verify(w => w.Send("Message Body"), Times.Exactly(1));
            Assert.Equal(Result.Ok, result);
        }

        [Fact]
        public void whenCallingPostShouldCorrectlySetHeaderForTimeStamp()
        {
            _timeMock.Setup(t => t.GetTimeInMilliseconds()).Returns(123456);

            Uri uri = new System.Uri("http://www.google.com");
            Message message = new Message(uri, "Message Body");

            message.Headers.Add("header1", "value1");
            message.Headers.Add("content-type", "type");
            message.Headers.Add("header2", "value2");

            var result = _httpClient.Post(message);

            _webRequestMock.Verify(w => w.AddHeader("submitted", "123456"), Times.Exactly(1));
        }
    }
}
