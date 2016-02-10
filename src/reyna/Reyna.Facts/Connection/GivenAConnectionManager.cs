
using Moq;
using Reyna.Interfaces;
using System;
using Xunit;
namespace Reyna.Facts.Connection
{
    public class GivenAConnectionManager
    {
        private ConnectionManager connectionManager;

        private Mock<IPowerManager> powerManager;

        private Mock<IConnectionInfo> connectionInfo;

        private Mock<IPreferences> preferences;

        private Mock<IBlackoutTime> blackoutTime;

        public GivenAConnectionManager()
        {
            this.powerManager = new Mock<IPowerManager>();
            this.connectionInfo = new Mock<IConnectionInfo>();
            this.preferences = new Mock<IPreferences>();
            this.blackoutTime = new Mock<IBlackoutTime>();

            this.connectionManager = new ConnectionManager(this.preferences.Object, this.powerManager.Object, this.connectionInfo.Object, this.blackoutTime.Object);

            this.connectionInfo.SetupGet(c => c.Connected).Returns(true);
            this.preferences.SetupGet(p => p.OnChargeBlackout).Returns(false);
            this.powerManager.Setup(p => p.IsPowerLineConnected()).Returns(false);
            this.preferences.SetupGet(p => p.OffChargeBlackout).Returns(false);
            this.preferences.SetupGet(p => p.RoamingBlackout).Returns(false);
            this.blackoutTime.Setup(b => b.CanSendAtTime(It.IsAny<DateTime>(), It.IsAny<String>())).Returns(true);
        }

        [Fact]
        public void whenCallingCanSendAndNetworkIsConnectedShouldReturnOk()
        {
            Result result = this.connectionManager.CanSend;
            Assert.Equal(Result.Ok, result);
        }

        [Fact]
        public void whenCallingCanSendAndNetworkIsNotConnectedShouldReturnNotConnected()
        {
            this.connectionInfo.SetupGet(c => c.Connected).Returns(false);
            Result result = this.connectionManager.CanSend;
            Assert.Equal(Result.NotConnected, result);
        }

        [Fact]
        public void whenCallingCanSendAndWwanBlackoutRangeIsNullShouldcallSaveCellularDataAsWwanForBackwardsCompatibility()
        {
            this.preferences.SetupGet(p => p.WwanBlackoutRange).Returns((String)null);
            var result = this.connectionManager.CanSend;
            this.preferences.Verify(p => p.SaveCellularDataAsWwanForBackwardsCompatibility(), Times.Exactly(1));
        }

        [Fact]
        public void whenCallingCanSendAndWwanBlackoutRangeIsNotNullShouldNotCallSaveCellularDataAsWwanForBackwardsCompatibility()
        {
            this.preferences.SetupGet(p => p.WwanBlackoutRange).Returns("its a string");
            var result = this.connectionManager.CanSend;
            this.preferences.Verify(p => p.SaveCellularDataAsWwanForBackwardsCompatibility(), Times.Never);
        }

        [Fact]
        public void whenCallingCanSendAndIsOnChargeBlackoutAndBatterIsChargingShouldReturnBlackout()
        {
            this.preferences.SetupGet(p => p.OnChargeBlackout).Returns(true);
            this.powerManager.Setup(p => p.IsPowerLineConnected()).Returns(true);

            var result = this.connectionManager.CanSend;

            Assert.Equal(Result.Blackout, result);
        }

        [Fact]
        public void whenCallingCanSendAndIsOffChargeBlackoutAndNotChargingShouldReturnBlackout()
        {
            this.preferences.SetupGet(p => p.OffChargeBlackout).Returns(true);
            this.powerManager.Setup(p => p.IsPowerLineConnected()).Returns(false);

            var result = this.connectionManager.CanSend;

            Assert.Equal(Result.Blackout, result);
        }

        [Fact]
        public void whenCallingCanSendAndIsRoamingBlackoutAndDeviceRoamingShouldReturnBlackout()
        {
            this.preferences.SetupGet(p => p.RoamingBlackout).Returns(true);
            this.connectionInfo.SetupGet(c => c.Roaming).Returns(true);

            var result = this.connectionManager.CanSend;

            Assert.Equal(Result.Blackout, result);
        }

        [Fact]
        public void whenCallingCanSendAndIsInWifiBlackoutTimeAndConnectedToWifiShouldReturnBlackout()
        {
            this.blackoutTime.Setup(b => b.CanSendAtTime(It.IsAny<DateTime>(), It.IsAny<String>())).Returns(false);
            this.connectionInfo.SetupGet(c => c.Wifi).Returns(true);
            
            var result = this.connectionManager.CanSend;

            Assert.Equal(Result.Blackout, result);
        }

        [Fact]
        public void whenCallingCanSendAndIsInMobileBlackoutTimeAndConnectedToMobileShouldReturnBlackout()
        {
            this.blackoutTime.Setup(b => b.CanSendAtTime(It.IsAny<DateTime>(), It.IsAny<String>())).Returns(false);
            this.connectionInfo.SetupGet(c => c.Mobile).Returns(true);

            var result = this.connectionManager.CanSend;

            Assert.Equal(Result.Blackout, result);
        }
    }
}
