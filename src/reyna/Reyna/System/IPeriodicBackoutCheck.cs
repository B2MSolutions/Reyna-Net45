namespace Reyna
{
    internal interface IPeriodicBackoutCheck
    {
        void SetPeriodicalTasksKeyName(string key);

        void Record(string task);

        bool IsTimeElapsed(string task, long periodInMilliseconds);
    }
}
