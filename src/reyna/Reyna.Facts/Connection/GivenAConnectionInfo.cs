using System;
using System.Net.NetworkInformation;
using MbnApi;
using Moq;
using Xunit;

namespace Reyna.Facts
{
    public class GivenAConnectionInfo
    {
        public GivenAConnectionInfo()
        {
            _mockNetworkInterfaceFactory = new Mock<INetworkInterfaceWrapperFactory>();

            _mockMobileNetworkInterface = new Mock<INetworkInterfaceWrapper>();
            _mockMobileNetworkInterface.Setup(ni => ni.NetworkInterfaceType).Returns(NetworkInterfaceType.Wwanpp);
            _mockMobileNetworkInterface.Setup(ni => ni.OperationalStatus).Returns(OperationalStatus.Down);

            _mockWirelessNetworkInterface = new Mock<INetworkInterfaceWrapper>();
            _mockWirelessNetworkInterface.Setup(ni => ni.NetworkInterfaceType).Returns(NetworkInterfaceType.Wireless80211);
            _mockWirelessNetworkInterface.Setup(ni => ni.OperationalStatus).Returns(OperationalStatus.Down);

            _mockNetworkInterfaceFactory.Setup(nc => nc.GetAllNetworkInterfaces()).Returns(new[] { _mockWirelessNetworkInterface.Object, _mockMobileNetworkInterface.Object });

            _mockMbnRegistration = new Mock<IMbnRegistration>();
            var mockMbnInterface = _mockMbnRegistration.As<IMbnInterface>();

            _mockMbnInterfaceManager = new Mock<IMbnInterfaceManagerWrapper>();
            _mockMbnInterfaceManager.Setup(m => m.MobileInterfaces).Returns(new []{ mockMbnInterface.Object });

            ConnectionInfo = new ConnectionInfo(_mockNetworkInterfaceFactory.Object, _mockMbnInterfaceManager.Object);
        }
        
        private readonly Mock<INetworkInterfaceWrapperFactory> _mockNetworkInterfaceFactory;
        private readonly Mock<INetworkInterfaceWrapper> _mockWirelessNetworkInterface;
        private readonly Mock<INetworkInterfaceWrapper> _mockMobileNetworkInterface;
        private readonly Mock<IMbnInterfaceManagerWrapper> _mockMbnInterfaceManager;
        private readonly Mock<IMbnRegistration> _mockMbnRegistration;

        private ConnectionInfo ConnectionInfo { get; set; }

        [Fact]
        public void Construction()
        {
            Assert.NotNull(ConnectionInfo);
        }

        [Theory]
        [InlineData(true, OperationalStatus.Up)]
        [InlineData(false, OperationalStatus.Down)]
        public void WhenGettingConnectedShouldReturnExpected(bool expected, OperationalStatus status)
        {
            _mockMobileNetworkInterface.Setup(ni => ni.OperationalStatus).Returns(status);

            Assert.Equal(expected, ConnectionInfo.Connected);
        }

        [Theory]
        [InlineData(true, OperationalStatus.Up)]
        [InlineData(false, OperationalStatus.Down)]
        public void WhenGettingMobileShouldReturnExpected(bool expected, OperationalStatus status)
        {
            _mockMobileNetworkInterface.Setup(ni => ni.OperationalStatus).Returns(status);

            Assert.Equal(expected, ConnectionInfo.Mobile);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void WhenGettingRoamingShouldReturnExpected(bool expected)
        {
            _mockMbnRegistration.Setup(mi => mi.GetRegisterState())
                .Returns(expected
                    ? MBN_REGISTER_STATE.MBN_REGISTER_STATE_ROAMING
                    : MBN_REGISTER_STATE.MBN_REGISTER_STATE_NONE);

            Assert.Equal(expected, ConnectionInfo.Roaming);
        }

        [Theory]
        [InlineData(true, OperationalStatus.Up)]
        [InlineData(false, OperationalStatus.Down)]
        public void WhenGettingWifiShouldReturnExpected(bool expected, OperationalStatus status)
        {
            _mockWirelessNetworkInterface.Setup(ni => ni.OperationalStatus).Returns(status);

            Assert.Equal(expected, ConnectionInfo.Wifi);
        }

        [Theory]
        [InlineData(NetworkInterfaceType.Wman)]
        [InlineData(NetworkInterfaceType.Wwanpp)]
        [InlineData(NetworkInterfaceType.Wwanpp2)]
        public void WhenGettingMobileShouldWorkForEachTypeOfMobileNetworkInterface(NetworkInterfaceType type)
        {
            _mockMobileNetworkInterface.Setup(ni => ni.NetworkInterfaceType).Returns(type);
            _mockMobileNetworkInterface.Setup(ni => ni.OperationalStatus).Returns(OperationalStatus.Up);

            Assert.True(ConnectionInfo.Mobile);
        }

        [Fact]
        public void WhenGettingConnectedAndThrowsShouldCatchAndReturnFalse()
        {
            _mockMobileNetworkInterface.Setup(ni => ni.OperationalStatus).Throws(new Exception());

            Assert.False(ConnectionInfo.Connected);
        }

        [Fact]
        public void WhenGettingMobileAndThrowsShouldCatchAndReturnFalse()
        {
            _mockMobileNetworkInterface.Setup(ni => ni.OperationalStatus).Throws(new Exception());
            
            Assert.False(ConnectionInfo.Mobile);
        }

        [Fact]
        public void WhenGettingRoamingAndThrowsShouldCatchAndReturnFalse()
        {
            _mockMbnRegistration.Setup(mi => mi.GetRegisterState()).Throws(new Exception());

            Assert.False(ConnectionInfo.Roaming);
        }

        [Fact]
        public void WhenGettingWifiAndThrowsShouldCatchAndReturnFalse()
        {
            _mockWirelessNetworkInterface.Setup(ni => ni.OperationalStatus).Throws(new Exception());

            Assert.False(ConnectionInfo.Wifi);
        }
    }
}
