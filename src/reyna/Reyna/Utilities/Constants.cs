namespace Reyna
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public static class Constants
    {
        public static class Injection
        {
            public static readonly string VOLATILE_STORE = "VolatileStore";
            public static readonly string SQLITE_STORE = "SQLiteStore";
            public static readonly string NETWORK_WAIT_HANDLE = "NetworkWaitHandle";
            public static readonly string STORE_WAIT_HANDLE = "StoreWaitHandle";
            public static readonly string FORWARD_WAIT_HANDLE = "ForwardWaitHandle";
            public static readonly string STORE_SERVICE = "StoreService";
            public static readonly string FORWARD_SERVICE = "ForwardService";
            public static readonly string BATCH_PROVIDER = "BatchProvider";
            public static readonly string MESSAGE_PROVIDER = "MessageProvider";
        }
    }
}
