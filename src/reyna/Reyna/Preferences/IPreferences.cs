
using System;

namespace Reyna
{
    public interface IPreferences
    {
        TimeRange CellularDataBlackout { get; }
        bool OffChargeBlackout { get; }
        bool OnChargeBlackout { get; }
        void ResetCellularDataBlackout();
        void ResetOffChargeBlackout();
        void ResetOnChargeBlackout();
        void ResetRoamingBlackout();
        void ResetWlanBlackoutRange();
        void ResetWwanBlackoutRange();
        bool RoamingBlackout { get; }
        void SetCellularDataBlackout(TimeRange range);
        void SetOffChargeBlackout(bool value);
        void SetOnChargeBlackout(bool value);
        void SetRoamingBlackout(bool value);
        void SetWlanBlackoutRange(string range);
        void SetWwanBlackoutRange(string range);
        string WlanBlackoutRange { get; }
        string WwanBlackoutRange { get; }
        long StorageSizeLimit { get; }
        void ResetStorageSizeLimit();
        void SetStorageSizeLimit(long limit);
        int ForwardServiceTemporaryErrorBackout {get; }
        int ForwardServiceMessageBackout { get; }
        void SaveCellularDataAsWwanForBackwardsCompatibility();
        Uri BatchUploadUrl { get; }
        bool BatchUpload { get; }
        long BatchUploadCheckInterval { get; }
        bool BatchUploadCheckIntervalEnabled { get; }
        void SaveBatchUpload(bool value);
        void SaveBatchUploadUrl(Uri url);
        void SaveBatchUploadInterval(long interval);
        void SaveBatchUploadIntervalEnabled(bool enabled);
    }
}
