namespace Reyna.Interfaces
{
    public interface ISystemNotifier
    {
        void NotifyOnNetworkConnect(string eventName);

        void ClearNotification(string eventName);
    }
}
