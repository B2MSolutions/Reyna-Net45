using System;
using System.Net.NetworkInformation;
using Moq;
using Xunit;

namespace Reyna.Facts
{
    public class GivenAConnectionInfo
    {
        public GivenAConnectionInfo()
        {
            _mockConnectionCost = new Mock<IConnectionCost>();
            _mockNetworkInterface = new Mock<INetworkInterfaceWrapperFactory>();

            _mockMobileNetworkInterface = new Mock<INetworkInterfaceWrapper>();
            _mockMobileNetworkInterface.Setup(ni => ni.NetworkInterfaceType).Returns(NetworkInterfaceType.Wwanpp);
            _mockMobileNetworkInterface.Setup(ni => ni.OperationalStatus).Returns(OperationalStatus.Down);

            _mockWirelessNetworkInterface = new Mock<INetworkInterfaceWrapper>();
            _mockWirelessNetworkInterface.Setup(ni => ni.NetworkInterfaceType).Returns(NetworkInterfaceType.Wireless80211);
            _mockWirelessNetworkInterface.Setup(ni => ni.OperationalStatus).Returns(OperationalStatus.Down);

            _mockNetworkInterface.Setup(nc => nc.GetAllNetworkInterfaces()).Returns(new[] { _mockWirelessNetworkInterface.Object, _mockMobileNetworkInterface.Object });

            ConnectionInfo = new ConnectionInfo(_mockConnectionCost.Object, _mockNetworkInterface.Object);
        }

        private readonly Mock<IConnectionCost> _mockConnectionCost;
        private readonly Mock<INetworkInterfaceWrapperFactory> _mockNetworkInterface;
        private readonly Mock<INetworkInterfaceWrapper> _mockWirelessNetworkInterface;
        private readonly Mock<INetworkInterfaceWrapper> _mockMobileNetworkInterface;

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
            _mockConnectionCost.Setup(ni => ni.Roaming).Returns(expected);

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
            _mockConnectionCost.Setup(ni => ni.Roaming).Throws(new Exception());

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
