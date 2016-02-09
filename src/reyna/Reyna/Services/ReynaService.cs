using Microsoft.Practices.Unity;

namespace Reyna
{
    using Interfaces;

    public sealed class ReynaService : IReyna
    {
        internal const long MinimumStorageLimit = 1867776; // 1Mb 800Kb

        internal IRepository VolatileStore { get; set; }
        internal IRepository PersistentStore { get; set; }
        internal IHttpClient HttpClient { get; set; }
        internal IPreferences Preferences { get; set; }
        internal IEncryptionChecker EncryptionChecker { get; set; }
        internal IStoreService StoreService { get; set; }
        internal IForwardService ForwardService { get; set; }
        internal INetworkStateService NetworkStateService { get; set; }
        internal IReynaLogger Logger { get; set; }
        
        internal byte[] Password { get; set; }

        public ReynaService() : this(null)
        {
        }

        public ReynaService(byte[] password) : this(password, UnityHelper.GetContainer())
        {
        }

        internal ReynaService(byte[] password, IUnityContainer container)
        {
            this.HttpClient = container.Resolve<IHttpClient>();
            this.Preferences = container.Resolve<IPreferences>();
            this.VolatileStore = container.Resolve<IRepository>(Constants.Injection.VOLATILE_STORE);
            this.PersistentStore = container.Resolve<IRepository>(Constants.Injection.SQLITE_STORE);
            this.NetworkStateService = container.Resolve<INetworkStateService>();
            this.StoreService = container.Resolve<IStoreService>();
            this.ForwardService = container.Resolve<IForwardService>();
            this.EncryptionChecker = container.Resolve<IEncryptionChecker>();

            this.Logger = container.Resolve<IReynaLogger>();

            this.StoreService.Initialize(this.VolatileStore, this.PersistentStore);
            this.ForwardService.Initialize(this.PersistentStore, this.HttpClient, this.NetworkStateService, this.Preferences.ForwardServiceTemporaryErrorBackout, this.Preferences.ForwardServiceMessageBackout);
                                            
            this.Password = password;

            if (password != null)
            {
                this.PersistentStore.Password = password;
            }
        }

        public long StorageSizeLimit
        {
            get
            {
                return this.Preferences.StorageSizeLimit;
            }
        }

        public void SetStorageSizeLimit(long limit)
        {
            limit = limit < MinimumStorageLimit ? MinimumStorageLimit : limit;
            this.Preferences.SetStorageSizeLimit(limit);

            this.PersistentStore.Initialise();
            this.PersistentStore.ShrinkDb(limit);
        }

        public void ResetStorageSizeLimit()
        {
            this.Preferences.ResetStorageSizeLimit();
        }

        public void SetCellularDataBlackout(TimeRange timeRange)
        {
            this.Preferences.SetCellularDataBlackout(timeRange);
        }

        public void ResetCellularDataBlackout()
        {
            this.Preferences.ResetCellularDataBlackout();
        }

        public void SetWlanBlackoutRange(string range)
        {
            this.Preferences.SetWlanBlackoutRange(range);
        }

        public void SetWwanBlackoutRange(string range)
        {
            this.Preferences.SetWwanBlackoutRange(range);
        }

        public void SetRoamingBlackout(bool value)
        {
            this.Preferences.SetRoamingBlackout(value);
        }

        public void SetOnChargeBlackout(bool value)
        {
            this.Preferences.SetOnChargeBlackout(value);
        }

        public void SetOffChargeBlackout(bool value)
        {
            this.Preferences.SetOffChargeBlackout(value);
        }

        public void Start()
        {
            Logger.Info("Reyna.ReynaService Start enter");  
          
            if (this.Password != null && this.Password.Length > 0)
            {
                if (!this.EncryptionChecker.DbEncrypted())
                {
                    this.EncryptionChecker.EncryptDb(this.Password);
                }
            }

            this.StoreService.Start();
            this.ForwardService.Start();
            this.NetworkStateService.Start();

            Logger.Info("Reyna.ReynaService Start exit");   
        }

        public void Stop()
        {
            Logger.Info("Reyna.ReynaService Stop enter");  

            this.NetworkStateService.Stop();
            this.ForwardService.Stop();
            this.StoreService.Stop();
            
            Logger.Info("Reyna.ReynaService Stop exit");  
        }

        public void Put(IMessage message)
        {
            this.VolatileStore.Add(message);
        }

        public void EnableLogging(ILogDelegate logDelegate)
        {
            Logger.Initialise(logDelegate);   
            Logger.Info("ReynaService.EnableLogging");
        }

        public void Dispose()
        {
            if (this.NetworkStateService != null)
            {
                this.NetworkStateService.Dispose();
            }

            if (this.ForwardService != null)
            {
                this.ForwardService.Dispose();
            }

            if (this.StoreService != null)
            {
                this.StoreService.Dispose();
            }
        }
    }
}
