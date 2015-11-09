namespace Reyna
{
    using System;
    using System.Threading;
    using Reyna.Interfaces;

    internal sealed class ForwardService : ServiceBase, IForwardService
    {
        internal int TemporaryErrorMilliseconds { get; set; }
        internal int SleepMilliseconds { get; set; }
        internal IHttpClient HttpClient { get; set; }
        internal INetworkStateService NetworkState { get; set; }

        public ForwardService(IAutoResetEventAdapter waitHandle)
            : base(waitHandle, true)
        {
        }
        
        private bool _snoozing;

        protected override void ThreadStart()
        {
            while (!this.Terminate)
            {
                this.DoWork();
            }
        }

        internal void DoWork()
        {
            this.WaitHandle.WaitOne();
            IMessage message = null;

            while (!this.Terminate && !_snoozing && (message = this.SourceStore.Get()) != null)
            {
                var result = this.HttpClient.Post(message);
                if (result == Result.TemporaryError)
                {
                    // Schedule a retry, after a suitable snooze.

                    _snoozing = true;

                    var timer = new System.Timers.Timer{ AutoReset = false, Enabled = true, Interval = this.TemporaryErrorMilliseconds };
                    timer.Elapsed += (source, args) => { _snoozing = false; this.WaitHandle.Set(); };

                    break;
                }

                if (result == Result.Blackout || result == Result.NotConnected)
                {
                    break;
                }

                this.SourceStore.Remove();
                Thread.Sleep(this.SleepMilliseconds);
            }

            this.WaitHandle.Reset();
        }

        internal void OnNetworkConnected(object sender, EventArgs e)
        {
            if (this.Terminate)
            {
                return;
            }

            this.SignalWorkToDo();
        }

        public void Initialize(IRepository sourceStore, IHttpClient httpClient, INetworkStateService networkState, 
            int temporaryErrorMilliseconds, int sleepMilliseconds)
        {
            if (httpClient == null)
            {
                throw new ArgumentNullException("httpClient");
            }

            if (networkState == null)
            {
                throw new ArgumentNullException("networkState");
            }

            this.HttpClient = httpClient;
            this.NetworkState = networkState;

            this.TemporaryErrorMilliseconds = temporaryErrorMilliseconds;
            this.SleepMilliseconds = sleepMilliseconds;

            this.NetworkState.NetworkConnected += this.OnNetworkConnected;

            base.Initialize(sourceStore);
        }
    }
}
