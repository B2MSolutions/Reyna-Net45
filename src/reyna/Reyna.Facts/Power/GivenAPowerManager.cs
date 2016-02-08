using System.Windows.Forms;
using Moq;
using Reyna.Power;
using Xunit;

namespace Reyna.Facts
{
    public class GivenAPowerManager
    {
        private readonly PowerManager _powerManager;
        private readonly Mock<IPowerStatusWrapper> _mockPowerStatusWrapper;

        public GivenAPowerManager()
        {
            _mockPowerStatusWrapper = new Mock<IPowerStatusWrapper>();
            _powerManager = new PowerManager(_mockPowerStatusWrapper.Object);
        }

        [Fact]
        public void WhenCallingIsBatteryChargingAndBatteryIsChargingShouldReturnTrue()
        {
            _mockPowerStatusWrapper.SetupGet(ps => ps.PowerLineStatus).Returns(PowerLineStatus.Online);
            Assert.True(_powerManager.IsPowerLineConnected());
        }

        [Fact]
        public void WhenCallingIsBatteryChargingAndBatteryIsNotChargingShouldReturnFalse()
        {
            _mockPowerStatusWrapper.SetupGet(ps => ps.PowerLineStatus).Returns(PowerLineStatus.Offline);
            Assert.False(_powerManager.IsPowerLineConnected());
        }

        [Fact]
        public void WhenCallingIsBatteryChargingAndBatteryFailedToGetPowerStatusShouldReturnFalse()
        {
            _mockPowerStatusWrapper.SetupGet(ps => ps.PowerLineStatus).Returns(null);
            Assert.False(_powerManager.IsPowerLineConnected());
        }
    }
}
