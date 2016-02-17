using Moq;

namespace Reyna.Facts
{
    using Microsoft.Win32;
    using System;
    using Xunit;

    public class GivenAPreferences
    {
        private readonly Mock<IRegistry> _mockRegistry;

        public GivenAPreferences()
        {
            _mockRegistry = new Mock<IRegistry>();
            Preferences = new Preferences(_mockRegistry.Object);
        }

        public Preferences Preferences { get; set; }

        [Fact]
        public void WhenGettingCellularDataBlackoutAndThrowsShouldReturnNull()
        {
            _mockRegistry.Setup(
                r => r.GetDWord(Registry.LocalMachine, @"Software\Reyna", "DataBlackout:From", It.IsAny<int>()))
                .Returns(-3);
            _mockRegistry.Setup(
                r => r.GetDWord(Registry.LocalMachine, @"Software\Reyna", "DataBlackout:To", It.IsAny<int>()))
                .Returns(-3);
            var timeRange = Preferences.CellularDataBlackout;

            Assert.Null(timeRange);
        }

        [Fact]
        public void WhenSettingCellularDataBlackoutThenGetCellularDataBlackoutShouldReturnExpected()
        {
            var range = new TimeRange(new Time(11, 00), new Time(12, 01));
            _mockRegistry.Setup(
                r => r.GetDWord(Registry.LocalMachine, @"Software\Reyna", "DataBlackout:From", It.IsAny<int>()))
                .Returns(660);
            _mockRegistry.Setup(
                r => r.GetDWord(Registry.LocalMachine, @"Software\Reyna", "DataBlackout:To", It.IsAny<int>()))
                .Returns(721);

            var timeRange = Preferences.CellularDataBlackout;

            Assert.Equal(timeRange.From.MinuteOfDay, range.From.MinuteOfDay);
            Assert.Equal(timeRange.To.MinuteOfDay, range.To.MinuteOfDay);
        }

        [Fact]
        public void WhenResetCellularDataBlackoutShouldDeleteValuesFromRegistry()
        {
            Preferences.ResetCellularDataBlackout();

            _mockRegistry.Verify(r => r.DeleteValue(Registry.LocalMachine, @"Software\Reyna", "DataBlackout:To"));
            _mockRegistry.Verify(r => r.DeleteValue(Registry.LocalMachine, @"Software\Reyna", "DataBlackout:From"));
        }

        [Fact]
        public void WhenGetCellularDataBlackoutAndNotCorrectlySavedShouldReturnNull()
        {
            var range = new TimeRange(new Time(11, 00), new Time(12, 01));
            _mockRegistry.Setup(
                r => r.GetDWord(Registry.LocalMachine, @"Software\Reyna", "DataBlackout:From", It.IsAny<int>()))
                .Returns(-3);
            _mockRegistry.Setup(
                r => r.GetDWord(Registry.LocalMachine, @"Software\Reyna", "DataBlackout:To", It.IsAny<int>()))
                .Returns(721);

            var timeRange = Preferences.CellularDataBlackout;

            Assert.Null(timeRange);
        }

        [Fact]
        public void WhenSettingWlanBlackoutRangeThenGetWlanBlackoutRangeShouldReturnExpected()
        {
            _mockRegistry.Setup(
                r => r.GetString(Registry.LocalMachine, @"Software\Reyna", "WlanBlackoutRange", It.IsAny<string>()))
                .Returns("00:00-00:01");

            var actual = Preferences.WlanBlackoutRange;

            Assert.Equal("00:00-00:01", actual);
            Assert.Equal("00:00-00:01", actual);
            
            _mockRegistry.Setup(
                r => r.GetString(Registry.LocalMachine, @"Software\Reyna", "WlanBlackoutRange", It.IsAny<string>()))
                .Returns("00:00-00:01,01:00-01:30");

            actual = Preferences.WlanBlackoutRange;

            Assert.Equal("00:00-00:01,01:00-01:30", actual);
            Assert.Equal("00:00-00:01,01:00-01:30", actual);
            
            _mockRegistry.Setup(
                r => r.GetString(Registry.LocalMachine, @"Software\Reyna", "WlanBlackoutRange", It.IsAny<string>()))
                .Returns("00:00-00:01,01:00-01:30,11:23-18:20");

            actual = Preferences.WlanBlackoutRange;

            Assert.Equal("00:00-00:01,01:00-01:30,11:23-18:20", actual);
            Assert.Equal("00:00-00:01,01:00-01:30,11:23-18:20", actual);
        }

        [Fact]
        public void WhenResetWlanBlackoutRangeShouldDeleteValuesFromRegistry()
        {
            Preferences.ResetWlanBlackoutRange();

            _mockRegistry.Verify(r => r.DeleteValue(Registry.LocalMachine, @"Software\Reyna", "WlanBlackoutRange"));
        }

        [Fact]
        public void WhenSettingWlanBlackoutRangeWithInvalidRangeShouldResetIt()
        {
            Preferences.SetWlanBlackoutRange("00");

            var range = Preferences.WlanBlackoutRange;

            Assert.Null(range);
            _mockRegistry.Verify(r => r.DeleteValue(Registry.LocalMachine, @"Software\Reyna", "WlanBlackoutRange"));
        }

        [Fact]
        public void WhenSettingWwanBlackoutRangeWithInvalidRangeShouldResetIt()
        {
            Preferences.SetWwanBlackoutRange("00");

            var range = Preferences.WwanBlackoutRange;

            Assert.Null(range);
            _mockRegistry.Verify(r => r.DeleteValue(Registry.LocalMachine, @"Software\Reyna", "WwanBlackoutRange"));
        }

        [Fact]
        public void WhenNoWlanBlackoutRangeThenGetWlanBlackoutRangeShouldReturnNull()
        {
            var range = Preferences.WlanBlackoutRange;

            Assert.Null(range);
        }

        [Fact]
        public void WhenNoWwanBlackoutRangeThenGetWlanBlackoutRangeShouldReturnNull()
        {
            var range = Preferences.WwanBlackoutRange;

            Assert.Null(range);
        }

        [Fact]
        public void WhenGetWlanBlackoutRangeAndNotCorrectlySavedShouldReturnNull()
        {
            var range = Preferences.WlanBlackoutRange;

            Assert.Null(range);
        }

        [Fact]
        public void WhenSettingWwanBlackoutRangeThenGetWwanBlackoutRangeShouldReturnExpected()
        {
            _mockRegistry.Setup(
                r => r.GetString(Registry.LocalMachine, @"Software\Reyna", "WwanBlackoutRange", It.IsAny<string>()))
                .Returns("00:00-00:01");

            var actual = Preferences.WwanBlackoutRange;

            Assert.Equal("00:00-00:01", actual);
            Assert.Equal("00:00-00:01", actual);
        }

        [Fact]
        public void WhenResetWwanBlackoutRangeShouldDeleteValuesFromRegistry()
        {
            Preferences.ResetWwanBlackoutRange();

            _mockRegistry.Verify(r => r.DeleteValue(Registry.LocalMachine, @"Software\Reyna", "WwanBlackoutRange"));
        }

        [Fact]
        public void WhenGetWwanBlackoutRangeAndNotCorrectlySavedShouldReturnNull()
        {
            var range = Preferences.WwanBlackoutRange;

            Assert.Null(range);
        }

        [Fact]
        public void WhenSettingRoamingBlackoutShouldSetValueInRegistry()
        {
            Preferences.SetRoamingBlackout(true);
            _mockRegistry.Verify(r => r.SetDWord(Registry.LocalMachine, @"Software\Reyna", "RoamingBlackout", 1));
        }

        [Fact]
        public void WhenSettingOffChargeBlackoutShouldSetValueInRegistry()
        {
            Preferences.SetOffChargeBlackout(true);
            _mockRegistry.Verify(r => r.SetDWord(Registry.LocalMachine, @"Software\Reyna", "OffChargeBlackout", 1));
        }

        [Fact]
        public void WhenSettingOnChargeBlackoutShouldSetValueInRegistry()
        {
            Preferences.SetOnChargeBlackout(true);
            _mockRegistry.Verify(r => r.SetDWord(Registry.LocalMachine, @"Software\Reyna", "OnChargeBlackout", 1));
        }

        [Fact]
        public void WhenGettingRoamingBlackoutShouldGetValueFromRegistry()
        {
            _mockRegistry.Setup(r => r.GetDWord(Registry.LocalMachine, @"Software\Reyna", "RoamingBlackout", 1)).Returns(1);

            var result = Preferences.RoamingBlackout;

            _mockRegistry.Verify(r => r.GetDWord(Registry.LocalMachine, @"Software\Reyna", "RoamingBlackout", 1));
            Assert.True(result);
        }

        [Fact]
        public void WhenGettingOffChargeBlackoutShouldGetValueFromRegistry()
        {
            _mockRegistry.Setup(r => r.GetDWord(Registry.LocalMachine, @"Software\Reyna", "OffChargeBlackout", 0)).Returns(1);

            var result = Preferences.OffChargeBlackout;

            _mockRegistry.Verify(r => r.GetDWord(Registry.LocalMachine, @"Software\Reyna", "OffChargeBlackout", 0));
            Assert.True(result);
        }

        [Fact]
        public void WhenGettingOnChargeBlackoutShouldGetValueFromRegistry()
        {
            _mockRegistry.Setup(r => r.GetDWord(Registry.LocalMachine, @"Software\Reyna", "OnChargeBlackout", 0)).Returns(1);

            var result = Preferences.OnChargeBlackout;

            _mockRegistry.Verify(r => r.GetDWord(Registry.LocalMachine, @"Software\Reyna", "OnChargeBlackout", 0));
            Assert.True(result);
        }

        [Fact]
        public void WhenResetOnChargeBlackoutShouldDeleteValuesFromRegistry()
        {
            Preferences.ResetOnChargeBlackout();

            _mockRegistry.Verify(r => r.DeleteValue(Registry.LocalMachine, @"Software\Reyna", "OnChargeBlackout"));
        }

        [Fact]
        public void WhenGetOnChargeBlackoutAndNotCorrectlySavedReturnFalse()
        {
            var chargingBlackout = Preferences.OnChargeBlackout;

            Assert.False(chargingBlackout);
        }

        [Fact]
        public void WhenResetOffChargeBlackoutShouldDeleteValuesFromRegistry()
        {
            Preferences.ResetOffChargeBlackout();
            
            _mockRegistry.Verify(r => r.DeleteValue(Registry.LocalMachine, @"Software\Reyna", "OffChargeBlackout"));
        }

        [Fact]
        public void WhenGetOffChargeBlackoutAndNotCorrectlySavedReturnFalse()
        {
            var dischargingBlackout = Preferences.OffChargeBlackout;

            Assert.False(dischargingBlackout);
        }

        [Fact]
        public void IsBlackoutRangeValidShouldReturnExpected()
        {
            Assert.True(Preferences.IsBlackoutRangeValid("00:00-02:30"));
            Assert.True(Preferences.IsBlackoutRangeValid("00:00-02:30,03:30-06:00"));
            Assert.True(Preferences.IsBlackoutRangeValid("00:00-02:30,03:30-06:00,07:00-07:01"));

            Assert.False(Preferences.IsBlackoutRangeValid(null));
            Assert.False(Preferences.IsBlackoutRangeValid(string.Empty));
            Assert.False(Preferences.IsBlackoutRangeValid("00:00"));
            Assert.False(Preferences.IsBlackoutRangeValid("1:00"));
            Assert.False(Preferences.IsBlackoutRangeValid("1:0002:00"));
            Assert.False(Preferences.IsBlackoutRangeValid("1"));
            Assert.False(Preferences.IsBlackoutRangeValid("00:10-"));
            Assert.False(Preferences.IsBlackoutRangeValid("00:10-1"));
            Assert.False(Preferences.IsBlackoutRangeValid("00:00-02:30-15:42"));
            Assert.False(Preferences.IsBlackoutRangeValid("13:00 - 21:00"));
            Assert.False(Preferences.IsBlackoutRangeValid("1300-21:00"));
        }
        
        [Theory,
        InlineData(true),
        InlineData(false)]
        public void WhenCallingSaveBatchUpoadShouldSetRegistryValue(bool value)
        {
            Preferences.SaveBatchUpload(value);
            _mockRegistry.Verify(r => r.SetDWord(Registry.LocalMachine, @"Software\Reyna", "BatchUpload", value ? 1 : 0));
        }

        [Theory,
        InlineData(122),
        InlineData(123456)]
        public void WhenCallingSaveBatchUpoadCheckIntervalShouldSetRegistryValue(long value)
        {
            Preferences.SaveBatchUploadCheckInterval(value);
            _mockRegistry.Verify(r => r.SetQWord(Registry.LocalMachine, @"Software\Reyna", "BatchUploadInterval", value));
        }

        [Theory,
        InlineData("http://url1.com"),
        InlineData("http://url2.com")]
        public void WhenCallingSaveBatchUpoadUrlShouldSetRegistryValue(string value)
        {
            Uri uri = new Uri(value);
            Preferences.SaveBatchUploadUrl(uri);
            _mockRegistry.Verify(r => r.SetString(Registry.LocalMachine, @"Software\Reyna", "BatchUploadUri", uri.ToString()));
        }

        [Theory,
        InlineData(true),
        InlineData(false)]
        public void WhenCallingBatchUploadShouldReturnCorrectValue(bool value)
        {
            _mockRegistry.Setup(r => r.GetDWord(Registry.LocalMachine, @"Software\Reyna", "BatchUpload", 1)).Returns(value ? 1 : 0);
            Assert.Equal(value, Preferences.BatchUpload);
        }

        [Theory,
        InlineData(122),
        InlineData(123456)]
        public void WhennCallingBatchUploadIntervalShouldReturnCorrectValue(long value)
        {
            long sixHours = 6 * 60 * 60 * 1000;
            _mockRegistry.Setup(r => r.GetQWord(Registry.LocalMachine, @"Software\Reyna", "BatchUploadInterval", sixHours)).Returns(value);
            Assert.Equal(value, Preferences.BatchUploadCheckInterval);
        }

        [Theory,
        InlineData("http://url1.com"),
        InlineData("http://url2.com")]
        public void WhenCallingBatchUpoadUrlShouldReturnCorrectValue(string value)
        {
            Uri uri = new Uri(value);
            _mockRegistry.Setup(r => r.GetString(Registry.LocalMachine, @"Software\Reyna", "BatchUploadUri", string.Empty)).Returns(value);
            var result = Preferences.BatchUploadUrl;
            Assert.Equal(uri.ToString(), result.ToString());
        }

        [Fact]
        public void WhenCallingBatchUploadUriAndNoStringReturnedShouldReturnNull()
        {
            _mockRegistry.Setup(r => r.GetString(Registry.LocalMachine, @"Software\Reyna", "BatchUploadUri", string.Empty)).Returns(String.Empty);
            Assert.Null(Preferences.BatchUploadUrl);
        }
    }
}
