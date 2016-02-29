namespace Reyna
{
    using System;

    internal class RegistryPeriodicBackoutCheck : IPeriodicBackoutCheck
    {
        public RegistryPeriodicBackoutCheck(IRegistry registry)
        {
            Registry = registry;
            PeriodicalTasksKeyName = @"Software\Reyna\PeriodicBackoutCheck";
        }

        public void SetPeriodicalTasksKeyName(string key)
        {
            PeriodicalTasksKeyName = key;
        }

        private IRegistry Registry { get; set; }

        private string PeriodicalTasksKeyName { get; set; }

        public void Record(string task)
        {
            var epocInMilliseconds = GetEpocInMilliSeconds();
            Registry.SetQWord(Microsoft.Win32.Registry.LocalMachine, PeriodicalTasksKeyName, task, epocInMilliseconds);
        }

        public bool IsTimeElapsed(string task, long periodInMilliseconds)
        {
            var lastCheckedTime = Registry.GetQWord(Microsoft.Win32.Registry.LocalMachine, PeriodicalTasksKeyName, task, 0);
            var epocInMilliseconds = GetEpocInMilliSeconds();

            var elapsedPeriodInSeconds = epocInMilliseconds - lastCheckedTime;

            if (lastCheckedTime > epocInMilliseconds)
            {
                Record(task);
                return true;
            }

            return elapsedPeriodInSeconds >= periodInMilliseconds;
        }

        private long GetEpocInMilliSeconds()
        {
            var span = DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Local);
            return (long)span.TotalMilliseconds;
        }
    }
}
