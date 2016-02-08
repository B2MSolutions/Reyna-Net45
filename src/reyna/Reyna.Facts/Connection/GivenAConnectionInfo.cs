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
            _mockNetworkInterface = new Mock<INetworkInterfaceWrapper>();

            var mockMobileNetworkInterface = new Mock<INetworkInterfaceWrapper>();
            mockMobileNetworkInterface.Setup(ni => ni.NetworkInterfaceType).Returns(NetworkInterfaceType.Wwanpp);
            mockMobileNetworkInterface.Setup(ni => ni.OperationalStatus).Returns(OperationalStatus.Down);

            var mockWirelessNetworkInterface = new Mock<INetworkInterfaceWrapper>();
            mockWirelessNetworkInterface.Setup(ni => ni.NetworkInterfaceType).Returns(NetworkInterfaceType.Wireless80211);
            mockWirelessNetworkInterface.Setup(ni => ni.OperationalStatus).Returns(OperationalStatus.Up);

            _mockNetworkInterface.Setup(nc => nc.GetAllNetworkInterfaces()).Returns(new[] { mockWirelessNetworkInterface.Object, mockMobileNetworkInterface.Object });

            ConnectionInfo = new ConnectionInfo(_mockConnectionCost.Object, _mockNetworkInterface.Object);
        }

        private readonly Mock<IConnectionCost> _mockConnectionCost;
        private readonly Mock<INetworkInterfaceWrapper> _mockNetworkInterface;

        private ConnectionInfo ConnectionInfo { get; set; }

        [Fact]
        public void Construction()
        {
            Assert.NotNull(ConnectionInfo);
        }

        [Fact]
        public void WhenGettingConnectedShouldReturnExpected()
        {
            //TODO
        }

        [Fact]
        public void WhenGettingMobileShouldReturnExpected()
        {
            //TODO
        }

        [Fact]
        public void WhenGettingRoamingShouldReturnExpected()
        {
            //TODO
        }

        [Fact]
        public void WhenGettingWifiShouldReturnExpected()
        {
            //TODO
        }
    }
}
