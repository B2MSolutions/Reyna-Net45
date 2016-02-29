namespace Reyna.Facts
{
    using System;
    using Moq;
    using Xunit;

    public class GivenABatchConfiguration
    {
        private Mock<IPreferences> _mockPreferences;

        public GivenABatchConfiguration()
        {
            _mockPreferences = new Mock<IPreferences>();
            BatchConfiguration = new BatchConfiguration(_mockPreferences.Object);
        }

        private BatchConfiguration BatchConfiguration { get; set; }

        [Fact]
        public void WhenConstructingShouldNotThrow()
        {
            Assert.NotNull(BatchConfiguration);
        }

        [Fact]
        public void BatchMessagesSizeShouldBe300K()
        {
            Assert.Equal(300 * 1024, BatchConfiguration.BatchMessagesSize);
        }

        [Fact]
        public void BatchMessageCountShouldBe100()
        {
            Assert.Equal(100, BatchConfiguration.BatchMessageCount);
        }

        [Fact]
        public void SubmitIntervalShouldBeOneDay()
        {
            Assert.Equal(24 * 60 * 60 * 1000, BatchConfiguration.SubmitInterval);
        }

        [Fact]
        public void BatchUploadUrlShouldReturnFromPreferences()
        {
            _mockPreferences.Setup(p => p.BatchUploadUrl).Returns(new Uri("http://post2.net"));
            Assert.Equal("http://post2.net/", BatchConfiguration.BatchUrl.ToString());
        }

        [Fact]
        public void CheckIntervalShouldReturnFromPreferences()
        {
            _mockPreferences.Setup(p => p.BatchUploadCheckInterval).Returns(100);
            Assert.Equal(100, BatchConfiguration.CheckInterval);
        }

        [Fact]
        public void CheckIntervalEnabledShouldReturnFromPreferences()
        {
            _mockPreferences.Setup(p => p.BatchUploadCheckIntervalEnabled).Returns(true);
            Assert.Equal(true, BatchConfiguration.CheckIntervalEnabled);
        }
    }
}
