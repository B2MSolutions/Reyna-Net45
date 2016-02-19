namespace Reyna
{
    using System;
    using System.Text.RegularExpressions;

    public class Preferences : IPreferences
    {
        private readonly IRegistry _registry;

        private const string SubKey = @"Software\Reyna";
        private const string StorageSizeLimitKeyName = "StorageSizeLimit";
        private const string WlanBlackoutRangeKeyName = "WlanBlackoutRange";
        private const string WwanBlackoutRangeKeyName = "WwanBlackoutRange";
        private const string RoamingBlackoutKeyName = "RoamingBlackout";
        private const string OnChargeBlackoutKeyName = "OnChargeBlackout";
        private const string OffChargeBlackoutKeyName = "OffChargeBlackout";
        private const string DataBlackoutFromKeyName = "DataBlackout:From";
        private const string DataBlackoutToKeyName = "DataBlackout:To";
        private const string TemporaryErrorBackout = "TemporaryErrorBackout";
        private const string MessageBackout = "MessageBackout";
        private const string BatchUploadKeyName = "BatchUpload";
        private const string BatchUploadUriKeyName = "BatchUploadUri";
        private const string BatchUploadIntervalKeyName = "BatchUploadInterval";
        private const string BatchUploadIntervalEnabledKeyName = "BatchUploadIntervalEnabled";

        public Preferences(IRegistry registry)
        {
            _registry = registry;
        }

        public TimeRange CellularDataBlackout
        {
            get
            {
                try
                {
                    int minuteOfDayFrom = GetRegistryValue(DataBlackoutFromKeyName, -1);
                    int minuteOfDayTo = GetRegistryValue(DataBlackoutToKeyName, -1);
                    if (minuteOfDayFrom == -1 || minuteOfDayTo == -1)
                    {
                        return null;
                    }

                    Time from = new Time(minuteOfDayFrom);
                    Time to = new Time(minuteOfDayTo);
                    return new TimeRange(from, to);
                }
                catch (Exception)
                {
                }

                return null;
            }
        }

        public string WlanBlackoutRange
        {
            get
            {
                return GetRegistryValue(WlanBlackoutRangeKeyName, null);
            }
        }

        public string WwanBlackoutRange
        {
            get
            {
                return GetRegistryValue(WwanBlackoutRangeKeyName, null);
            }
        }

        public bool RoamingBlackout
        {
            get
            {
                return GetRegistryValue(RoamingBlackoutKeyName, true);
            }
        }

        public bool OnChargeBlackout
        {
            get
            {
                return GetRegistryValue(OnChargeBlackoutKeyName, false);
            }
        }

        public bool OffChargeBlackout
        {
            get
            {
                return GetRegistryValue(OffChargeBlackoutKeyName, false);
            }
        }

        public bool BatchUpload
        {
            get
            {
                return GetRegistryValue(BatchUploadKeyName, true);
            }
        }

        public Uri BatchUploadUrl
        {
            get
            {
                var url = GetRegistryValue(BatchUploadUriKeyName, string.Empty);
                if (string.IsNullOrEmpty(url))
                {
                    return null;
                }

                return new Uri(url);
            }
        }

        public long BatchUploadCheckInterval
        {
            get
            {
                long twentyFourHours = 24 * 60 * 60 * 1000;
                return GetRegistryValue(BatchUploadIntervalKeyName, twentyFourHours);
            }
        }

        public bool BatchUploadCheckIntervalEnabled
        {
            get
            {
                return GetRegistryValue(BatchUploadIntervalEnabledKeyName, false);
            }
        }

        public int ForwardServiceTemporaryErrorBackout
        {
            get
            {
                return GetRegistryValue(TemporaryErrorBackout, 5 * 60 * 1000);
            }
        }

        public int ForwardServiceMessageBackout
        {
            get
            {
                return GetRegistryValue(MessageBackout, 1000);
            }
        }

        public long StorageSizeLimit
        {
            get
            {
                return GetRegistryValue(StorageSizeLimitKeyName, (long)-1);
            }
        }

        public void SetCellularDataBlackout(TimeRange range)
        {
            SetRegistryValue(DataBlackoutFromKeyName, range.From.MinuteOfDay);
            SetRegistryValue(DataBlackoutToKeyName, range.To.MinuteOfDay);
        }

        public void ResetCellularDataBlackout()
        {
            DeleteRegistryValue(DataBlackoutFromKeyName);
            DeleteRegistryValue(DataBlackoutToKeyName);
        }

        public void SetWlanBlackoutRange(string range)
        {
            if (IsBlackoutRangeValid(range))
            {
                SetRegistryValue(WlanBlackoutRangeKeyName, range);
            }
            else
            {
                ResetWlanBlackoutRange();
            }
        }

        public void ResetWlanBlackoutRange()
        {
            DeleteRegistryValue(WlanBlackoutRangeKeyName);
        }

        public void SetWwanBlackoutRange(string range)
        {
            if (IsBlackoutRangeValid(range))
            {
                SetRegistryValue(WwanBlackoutRangeKeyName, range);
            }
            else
            {
                ResetWwanBlackoutRange();
            }
        }

        public void ResetWwanBlackoutRange()
        {
            DeleteRegistryValue(WwanBlackoutRangeKeyName);
        }

        public void SetRoamingBlackout(bool value)
        {
            SetRegistryValue(RoamingBlackoutKeyName, value);
        }

        public void ResetRoamingBlackout()
        {
            DeleteRegistryValue(RoamingBlackoutKeyName);
        }

        public void SetOnChargeBlackout(bool value)
        {
            SetRegistryValue(OnChargeBlackoutKeyName, value);
        }

        public void ResetOnChargeBlackout()
        {
            DeleteRegistryValue(OnChargeBlackoutKeyName);
        }

        public void SetOffChargeBlackout(bool value)
        {
            SetRegistryValue(OffChargeBlackoutKeyName, value);
        }

        public void ResetOffChargeBlackout()
        {
            DeleteRegistryValue(OffChargeBlackoutKeyName);
        }

        internal static bool IsBlackoutRangeValid(string ranges)
        {
            if (string.IsNullOrEmpty(ranges))
            {
                return false;
            }

            string[] splitRanges = ranges.Split(',');
            foreach (string range in splitRanges)
            {
                string regex = "^[0-9][0-9]:[0-9][0-9]-[0-9][0-9]:[0-9][0-9]$";
                if (!Regex.IsMatch(range, regex, RegexOptions.IgnoreCase))
                {
                    return false;
                }
            }

            return true;
        }

        public void SaveCellularDataAsWwanForBackwardsCompatibility()
        {
            TimeRange timeRange = CellularDataBlackout;
            if (timeRange != null)
            {
                int hourFrom = (int)Math.Floor((double)timeRange.From.MinuteOfDay / 60);
                int minuteFrom = timeRange.From.MinuteOfDay % 60;
                string blackoutFrom = ZeroPad(hourFrom) + ":" + ZeroPad(minuteFrom);

                int hourTo = (int)Math.Floor((double)timeRange.To.MinuteOfDay / 60);
                int minuteTo = timeRange.To.MinuteOfDay % 60;
                string blackoutTo = ZeroPad(hourTo) + ":" + ZeroPad(minuteTo);

                SetWwanBlackoutRange(blackoutFrom + "-" + blackoutTo);
            }
        }

        public void SaveBatchUpload(bool value)
        {
            SetRegistryValue(BatchUploadKeyName, value);
        }

        public void SaveBatchUploadUrl(Uri url)
        {
            SetRegistryValue(BatchUploadUriKeyName, url.ToString());
        }

        public void SaveBatchUploadInterval(long interval)
        {
            SetRegistryValue(BatchUploadIntervalKeyName, interval);
        }

        public void SaveBatchUploadIntervalEnabled(bool enabled)
        {
            SetRegistryValue(BatchUploadIntervalEnabledKeyName, enabled);
        }

        public void SetStorageSizeLimit(long limit)
        {
            SetRegistryValue(StorageSizeLimitKeyName, limit);
        }

        public void ResetStorageSizeLimit()
        {
            DeleteRegistryValue(StorageSizeLimitKeyName);
        }

        private long GetRegistryValue(string keyName, long defaultValue)
        {
            return _registry.GetQWord(Microsoft.Win32.Registry.LocalMachine, SubKey, keyName, defaultValue);
        }

        private int GetRegistryValue(string keyName, int defaultValue)
        {
            return _registry.GetDWord(Microsoft.Win32.Registry.LocalMachine, SubKey, keyName, defaultValue);
        }

        private bool GetRegistryValue(string keyName, bool defaultValue)
        {
            return _registry.GetDWord(Microsoft.Win32.Registry.LocalMachine, SubKey, keyName, defaultValue ? 1 : 0) == 1;
        }

        private string GetRegistryValue(string keyName, string defaultValue)
        {
            return _registry.GetString(Microsoft.Win32.Registry.LocalMachine, SubKey, keyName, defaultValue);
        }

        private void SetRegistryValue(string keyName, long value)
        {
            _registry.SetQWord(Microsoft.Win32.Registry.LocalMachine, SubKey, keyName, value);
        }

        private void SetRegistryValue(string keyName, string value)
        {
            _registry.SetString(Microsoft.Win32.Registry.LocalMachine, SubKey, keyName, value);
        }

        private void SetRegistryValue(string keyName, bool value)
        {
            _registry.SetDWord(Microsoft.Win32.Registry.LocalMachine, SubKey, keyName, value ? 1 : 0);
        }

        private void SetRegistryValue(string keyName, int value)
        {
            _registry.SetDWord(Microsoft.Win32.Registry.LocalMachine, SubKey, keyName, value);
        }

        private void DeleteRegistryValue(string keyName)
        {
            _registry.DeleteValue(Microsoft.Win32.Registry.LocalMachine, SubKey, keyName);
        }

        private static object ZeroPad(int numToPad)
        {
            return numToPad.ToString("D2");
        }
    }
}
