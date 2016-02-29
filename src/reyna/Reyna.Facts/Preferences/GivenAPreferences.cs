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

        [Fact]
        public void WhenSettingBatchUploadThenBatchUploadShouldReturnExpected()
        {
            _mockRegistry.Setup(r => r.GetDWord(Registry.LocalMachine, @"Software\Reyna", "BatchUpload", 1)).Returns(1);

            var batchUpload = Preferences.BatchUpload;

            Assert.True(batchUpload);

            _mockRegistry.Setup(r => r.GetDWord(Registry.LocalMachine, @"Software\Reyna", "BatchUpload", 1)).Returns(0);

            batchUpload = Preferences.BatchUpload;

            Assert.False(batchUpload);
        }

        [Fact]
        public void WhenGettingBatchUploadAndBatchUploadNeverSavedShouldReturnDefaults()
        {
            _mockRegistry.Setup(r => r.GetDWord(Registry.LocalMachine, @"Software\Reyna", "BatchUpload", 1)).Returns(1);

            var batchUpload = Preferences.BatchUpload;

            Assert.True(batchUpload);
        }

        [Fact]
        public void WhenSettingBatchUploadUrlThenBatchUploadUrlShouldReturnExpected()
        {
            _mockRegistry.Setup(r => r.GetString(Registry.LocalMachine, @"Software\Reyna", "BatchUploadUri", string.Empty)).Returns("http://post.net/");
            
            var batchUploadUrl = Preferences.BatchUploadUrl;

            Assert.Equal("http://post.net/", batchUploadUrl.ToString());
        }

        [Fact]
        public void WhenGettingBatchUploadUrlAndBatchUploadNeverSavedShouldReturnDefaults()
        {
            var batchUploadUrl = Preferences.BatchUploadUrl;

            Assert.Null(batchUploadUrl);
        }

        [Fact]
        public void WhenSettingBatchUploadCheckIntervalThenBatchUploadCheckIntervalShouldReturnExpected()
        {
            long sixHours = 6*60*60*1000;
            _mockRegistry.Setup(r => r.GetQWord(Registry.LocalMachine, @"Software\Reyna", "BatchUploadInterval", sixHours)).Returns(100);

            var batchUploadCheckInterval = Preferences.BatchUploadCheckInterval;

            Assert.Equal(100, batchUploadCheckInterval);
        }

        [Fact]
        public void WhenGettingBatchUploadCheckIntervalAndBatchUploadNeverSavedShouldReturnDefaults()
        {
            long sixHours = 6 * 60 * 60 * 1000;
            _mockRegistry.Setup(r => r.GetQWord(Registry.LocalMachine, @"Software\Reyna", "BatchUploadInterval", sixHours)).Returns(sixHours);

            var batchUploadCheckInterval = Preferences.BatchUploadCheckInterval;

            Assert.Equal(sixHours, batchUploadCheckInterval);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void WhenSettingBatchUploadCheckIntervalEnabledThenBatchUploadCheckIntervalShouldReturnExpected(bool expected)
        {
            _mockRegistry.Setup(r => r.GetDWord(Registry.LocalMachine, @"Software\Reyna", "BatchUploadIntervalEnabled", 0)).Returns(expected?1:0);

            var batchUploadCheckIntervalEnabled = Preferences.BatchUploadCheckIntervalEnabled;

            Assert.Equal(expected, batchUploadCheckIntervalEnabled);
        }

        [Fact]
        public void WhenGettingBatchUploadCheckIntervalEnabledAndBatchUploadNeverSavedShouldReturnDefaults()
        {
            var batchUploadCheckIntervalEnabled = Preferences.BatchUploadCheckIntervalEnabled;

            Assert.Equal(false, batchUploadCheckIntervalEnabled);
        }
    }
}
