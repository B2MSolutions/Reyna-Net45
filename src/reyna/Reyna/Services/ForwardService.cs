namespace Reyna
{
    using System;
    using System.Threading;
    using Interfaces;

    internal sealed class ForwardService : ServiceBase, IForwardService
    {
        internal int TemporaryErrorMilliseconds { get; set; }
        internal int SleepMilliseconds { get; set; }
        internal IHttpClient HttpClient { get; set; }
        internal INetworkStateService NetworkState { get; set; }
        internal IReynaLogger Logger { get; set; }
        internal IMessageProvider MessageProvider { get; set; }
        internal IPeriodicBackoutCheck PeriodicBackoutCheck { get; set; }

        public ForwardService(IAutoResetEventAdapter waitHandle, IReynaLogger logger)
            : base(waitHandle, true)
        {
            Logger = logger;
        }
        
        private bool _snoozing;

        protected override void ThreadStart()
        {
            while (!Terminate)
            {
                DoWork();
            }
        }

        internal void DoWork()
        {
            WaitHandle.WaitOne();
            IMessage message = null;
            Logger.Info("Reyna.ForwardService DoWork enter");
            while (!Terminate && !_snoozing && (message = SourceStore.Get()) != null)
            {
                var result = HttpClient.Post(message);
                if (result == Result.TemporaryError)
                {
                    // Schedule a retry, after a suitable snooze.

                    _snoozing = true;

                    var timer = new System.Timers.Timer{ AutoReset = false, Enabled = true, Interval = TemporaryErrorMilliseconds };
                    timer.Elapsed += (source, args) => { _snoozing = false; WaitHandle.Set(); };

                    break;
                }

                if (result == Result.Blackout || result == Result.NotConnected)
                {
                    break;
                }

                SourceStore.Remove();
                Thread.Sleep(SleepMilliseconds);
            }

            WaitHandle.Reset();
            Logger.Info("Reyna.ForwardService DoWork exit");
        }

        internal void OnNetworkConnected(object sender, EventArgs e)
        {
            Logger.Info("Reyna.ForwardService OnNetworkConnected");
           
            if (Terminate)
            {
                return;
            }

            SignalWorkToDo();
        }

        public void Initialize(IRepository sourceStore, IHttpClient httpClient, INetworkStateService networkState, 
            int temporaryErrorMilliseconds, int sleepMilliseconds, bool batchUpload)
        {
            Logger.Info("Reyna.ForwardService Initialize enter");
         
            if (httpClient == null)
            {
                throw new ArgumentNullException("httpClient");
            }

            if (networkState == null)
            {
                throw new ArgumentNullException("networkState");
            }

            HttpClient = httpClient;
            NetworkState = networkState;

            TemporaryErrorMilliseconds = temporaryErrorMilliseconds;
            SleepMilliseconds = sleepMilliseconds;

            NetworkState.NetworkConnected += OnNetworkConnected;

            if (batchUpload)
            {
                MessageProvider = new BatchProvider(sourceStore, PeriodicBackoutCheck, new BatchConfiguration(new Preferences(new Registry())));
            }
            else
            {
                MessageProvider = new MessageProvider(sourceStore);
            }

            Initialize(sourceStore);

            Logger.Info("Reyna.ForwardService Initialize exit");
        }
    }
}
