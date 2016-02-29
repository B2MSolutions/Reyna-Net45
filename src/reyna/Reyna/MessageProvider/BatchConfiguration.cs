namespace Reyna
{
    using System;

    internal class BatchConfiguration : IBatchConfiguration
    {
        private readonly IPreferences _preferences;

        public BatchConfiguration(IPreferences preferences)
        {
            _preferences = preferences;
        }

        public int BatchMessageCount
        {
            get
            {
                return 100;
            }
        }

        public long BatchMessagesSize
        {
            get
            {
                return 300 * 1024;
            }
        }

        public long CheckInterval
        {
            get
            {
                return _preferences.BatchUploadCheckInterval;
            }
        }

        public long SubmitInterval
        {
            get
            {
                return 24 * 60 * 60 * 1000;
            }
        }

        public Uri BatchUrl
        {
            get
            {
                return _preferences.BatchUploadUrl;
            }
        }

        public bool CheckIntervalEnabled
        {
            get { return _preferences.BatchUploadCheckIntervalEnabled; }
        }
    }
}
