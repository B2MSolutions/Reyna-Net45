
namespace Reyna
{
    public interface IPreferences
    {
        global::Reyna.TimeRange CellularDataBlackout { get; }
        bool OffChargeBlackout { get; }
        bool OnChargeBlackout { get; }
        void ResetCellularDataBlackout();
        void ResetOffChargeBlackout();
        void ResetOnChargeBlackout();
        void ResetRoamingBlackout();
        void ResetWlanBlackoutRange();
        void ResetWwanBlackoutRange();
        bool RoamingBlackout { get; }
        void SetCellularDataBlackout(global::Reyna.TimeRange range);
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
    }
}
