using System;
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
        
        public ReynaService(byte[] password = null) : this(password, UnityHelper.GetContainer())
        {
        }

        internal ReynaService(byte[] password, IUnityContainer container)
        {
            HttpClient = container.Resolve<IHttpClient>();
            Preferences = container.Resolve<IPreferences>();
            VolatileStore = container.Resolve<IRepository>(Constants.Injection.VOLATILE_STORE);
            PersistentStore = container.Resolve<IRepository>(Constants.Injection.SQLITE_STORE);
            NetworkStateService = container.Resolve<INetworkStateService>();
            StoreService = container.Resolve<IStoreService>();
            ForwardService = container.Resolve<IForwardService>();
            EncryptionChecker = container.Resolve<IEncryptionChecker>();

            Logger = container.Resolve<IReynaLogger>();

            StoreService.Initialize(VolatileStore, PersistentStore);
            ForwardService.Initialize(PersistentStore, HttpClient, NetworkStateService, Preferences.ForwardServiceTemporaryErrorBackout, Preferences.ForwardServiceMessageBackout, Preferences.BatchUpload);
                                            
            Password = password;

            if (password != null)
            {
                PersistentStore.Password = password;
            }
        }

        public long StorageSizeLimit
        {
            get
            {
                return Preferences.StorageSizeLimit;
            }
        }

        public void SetStorageSizeLimit(long limit)
        {
            limit = limit < MinimumStorageLimit ? MinimumStorageLimit : limit;
            Preferences.SetStorageSizeLimit(limit);

            PersistentStore.Initialise();
            PersistentStore.ShrinkDb(limit);
        }

        public void ResetStorageSizeLimit()
        {
            Preferences.ResetStorageSizeLimit();
        }

        public void SetCellularDataBlackout(TimeRange timeRange)
        {
            Preferences.SetCellularDataBlackout(timeRange);
        }

        public void ResetCellularDataBlackout()
        {
            Preferences.ResetCellularDataBlackout();
        }

        public void SetWlanBlackoutRange(string range)
        {
            Preferences.SetWlanBlackoutRange(range);
        }

        public void SetWwanBlackoutRange(string range)
        {
            Preferences.SetWwanBlackoutRange(range);
        }

        public void SetRoamingBlackout(bool value)
        {
            Preferences.SetRoamingBlackout(value);
        }

        public void SetOnChargeBlackout(bool value)
        {
            Preferences.SetOnChargeBlackout(value);
        }

        public void SetOffChargeBlackout(bool value)
        {
            Preferences.SetOffChargeBlackout(value);
        }

        public void SetBatchUploadConfiguration(bool value, Uri url, long interval)
        {
            Preferences.SaveBatchUpload(value);
            Preferences.SaveBatchUploadUrl(url);
            Preferences.SaveBatchUploadInterval(interval);
            if (interval >= 0)
            {
                Preferences.SaveBatchUploadIntervalEnabled(true);
            }
        }

        public void Start()
        {
            Logger.Info("Reyna.ReynaService Start enter");  
          
            if (Password != null && Password.Length > 0)
            {
                if (!EncryptionChecker.DbEncrypted())
                {
                    EncryptionChecker.EncryptDb(Password);
                }
            }

            StoreService.Start();
            ForwardService.Start();
            NetworkStateService.Start();

            Logger.Info("Reyna.ReynaService Start exit");   
        }

        public void Stop()
        {
            Logger.Info("Reyna.ReynaService Stop enter");  

            NetworkStateService.Stop();
            ForwardService.Stop();
            StoreService.Stop();
            
            Logger.Info("Reyna.ReynaService Stop exit");  
        }

        public void Put(IMessage message)
        {
            VolatileStore.Add(message);
        }

        public void EnableLogging(ILogDelegate logDelegate)
        {
            Logger.Initialise(logDelegate);   
            Logger.Info("ReynaService.EnableLogging");
        }

        public void Dispose()
        {
            if (NetworkStateService != null)
            {
                NetworkStateService.Dispose();
            }

            if (ForwardService != null)
            {
                ForwardService.Dispose();
            }

            if (StoreService != null)
            {
                StoreService.Dispose();
            }
        }
    }
}
