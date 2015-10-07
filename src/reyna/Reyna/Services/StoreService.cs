namespace Reyna
{
    using System;
    using Reyna.Interfaces;

    internal sealed class StoreService : ServiceBase, IStoreService
    {
        internal IRepository TargetStore { get; set; }
        internal IPreferences preferences;

        public StoreService( IAutoResetEventAdapter waitHandle, IPreferences preferences) : base(waitHandle, false)
        {
            this.preferences = preferences;
        }

        protected override void ThreadStart()
        {
            while (!this.Terminate)
            {
                this.WaitHandle.WaitOne();
                IMessage message = null;

                while ((message = this.SourceStore.Get()) != null)
                {
                    long storageSizeLimit = this.preferences.StorageSizeLimit;
                    if (storageSizeLimit == -1)
                    {
                        this.TargetStore.Add(message);
                    }
                    else
                    {
                        this.TargetStore.Add(message, storageSizeLimit);
                    }
                    
                    this.SourceStore.Remove();
                }

                this.WaitHandle.Reset();
            }
        }

        public void Initialize(IRepository sourceStore, IRepository targetStore)
        {
            if (targetStore == null)
            {
                throw new ArgumentNullException("targetStore");
            }

            this.TargetStore = targetStore;
            this.TargetStore.Initialise();
            base.Initialize(sourceStore);
        }
    }
}
