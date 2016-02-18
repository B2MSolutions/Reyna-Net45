namespace Reyna
{
    using System;
    using System.Threading;
    using Interfaces;

    internal sealed class ForwardService : ServiceBase, IForwardService
    {
        private const string PeriodicBackoutCheckTag = "ForwardService";
        private bool _snoozing;
        private readonly IBatchConfiguration _batchConfiguration;

        internal int TemporaryErrorMilliseconds { get; set; }
        internal int SleepMilliseconds { get; set; }
        internal IHttpClient HttpClient { get; set; }
        internal INetworkStateService NetworkState { get; set; }
        private IReynaLogger Logger { get; set; }
        internal IMessageProvider MessageProvider { get; set; }
        private IPeriodicBackoutCheck PeriodicBackoutCheck { get; set; }

        public ForwardService(IAutoResetEventAdapter waitHandle, IReynaLogger logger, IBatchConfiguration batchConfiguration, IPeriodicBackoutCheck periodicBackoutCheck)
            : base(waitHandle, true)
        {
            Logger = logger;
            _batchConfiguration = batchConfiguration;
            PeriodicBackoutCheck = periodicBackoutCheck;
        }

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
            Logger.Info("Reyna.ForwardService DoWork enter");

            if (CanSend)
            {
                IMessage message;
                while (!Terminate && !_snoozing && (message = MessageProvider.GetNext()) != null)
                {
                    var result = HttpClient.Post(message);
                    if (result == Result.TemporaryError)
                    {
                        // Schedule a retry, after a suitable snooze.

                        _snoozing = true;

                        var timer = new System.Timers.Timer
                        {
                            AutoReset = false,
                            Enabled = true,
                            Interval = TemporaryErrorMilliseconds
                        };
                        timer.Elapsed += (source, args) =>
                        {
                            _snoozing = false;
                            WaitHandle.Set();
                        };

                        break;
                    }

                    if (result == Result.Blackout || result == Result.NotConnected)
                    {
                        break;
                    }

                    MessageProvider.Delete(message);
                    Thread.Sleep(SleepMilliseconds);
                }

                MessageProvider.Close();
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
            PeriodicBackoutCheck.SetPeriodicalTasksKeyName(@"Software\Reyna\PeriodicBackoutCheck");

            if (batchUpload)
            {
                MessageProvider = new BatchProvider(sourceStore, PeriodicBackoutCheck, _batchConfiguration);
            }
            else
            {
                MessageProvider = new MessageProvider(sourceStore);
            }

            Initialize(sourceStore);

            Logger.Info("Reyna.ForwardService Initialize exit");
        }

        private bool CanSend
        {
            get
            {
                return MessageProvider.CanSend && PeriodicBackoutCheck.IsTimeElapsed(PeriodicBackoutCheckTag, TemporaryErrorMilliseconds);
            }
        }
    }
}
