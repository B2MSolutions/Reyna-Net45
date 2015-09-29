namespace Reyna
{
    using System;
    using System.Net;
    using Microsoft.Win32;
    using Reyna.Interfaces;
    using Microsoft.Practices.Unity;

    public sealed class ReynaService : IReyna
    {
        internal const long MinimumStorageLimit = 1867776; // 1Mb 800Kb

        internal IRepository VolatileStore { get; set; }
        internal IRepository PersistentStore { get; set; }
        internal IHttpClient HttpClient { get; set; }
        internal IPreferences Preferences { get; set; }
        internal IEncryptionChecker EncryptionChecker { get; set; }
        internal IService StoreService { get; set; }
        internal IService ForwardService { get; set; }
        internal INetworkStateService NetworkStateService { get; set; }
        internal IWaitHandle StoreWaitHandle { get; set; }
        internal IWaitHandle ForwardWaitHandle { get; set; }
        internal IWaitHandle NetworkWaitHandle { get; set; }
        internal ISystemNotifier SystemNotifier { get; set; }
        internal byte[] Password { get; set; }

        public ReynaService() : this(null, null)
        {
        }

        public ReynaService(byte[] password, ICertificatePolicy certificatePolicy) : this(password, certificatePolicy, UnityHelper.GetContainer())
        {
        }

        internal ReynaService(byte[] password, ICertificatePolicy certificatePolicy, IUnityContainer container)
        {
            this.HttpClient = container.Resolve<IHttpClient>();
            this.Preferences = container.Resolve<IPreferences>();
            this.VolatileStore = container.Resolve<IRepository>(Constants.Injection.VOLATILE_STORE);
            this.PersistentStore = container.Resolve<IRepository>(Constants.Injection.SQLITE_STORE);
            this.SystemNotifier = container.Resolve<ISystemNotifier>();

            this.NetworkWaitHandle = container.Resolve<IWaitHandle>(Constants.Injection.NETWORK_WAIT_HANDLE,
                new ResolverOverride[]
                                   {
                                       new ParameterOverride("initialState", false), 
                                       new ParameterOverride("name", Reyna.NetworkStateService.NetworkConnectedNamedEvent)
                                   });
            
            this.NetworkStateService = container.Resolve<INetworkStateService>(
                new ResolverOverride[]
                                   {
                                       new ParameterOverride("systemNotifier", this.SystemNotifier), 
                                       new ParameterOverride("networkWaitHandle",this.NetworkWaitHandle)
                                   });

            this.StoreWaitHandle = container.Resolve<IWaitHandle>(Constants.Injection.STORE_WAIT_HANDLE,
                new ResolverOverride[]
                                    { 
                                        new ParameterOverride("initialState", false)
                                    });


            this.ForwardWaitHandle = container.Resolve<IWaitHandle>(Constants.Injection.FORWARD_WAIT_HANDLE,
                new ResolverOverride[]
                                    { 
                                        new ParameterOverride("initialState", false)
                                    });

            this.ForwardWaitHandle = container.Resolve<IWaitHandle>(Constants.Injection.FORWARD_WAIT_HANDLE,
                new ResolverOverride[]
                                    { 
                                        new ParameterOverride("initialState", false)
                                    });

            this.StoreService = container.Resolve<IService>(Constants.Injection.STORE_SERVICE,
                new ResolverOverride[]
                                    {
                                        new ParameterOverride("sourceStore", this.VolatileStore),
                                        new ParameterOverride("tagetStore", this.PersistentStore),
                                        new ParameterOverride("waitHandle", this.StoreWaitHandle)
                                    });

            this.ForwardService = container.Resolve<IService>(Constants.Injection.FORWARD_SERVICE,
                new ResolverOverride[]
                                    {
                                        new ParameterOverride("sourceStore", this.PersistentStore),
                                        new ParameterOverride("httpClient", this.HttpClient),
                                        new ParameterOverride("networkState", this.NetworkStateService),
                                        new ParameterOverride("waitHandle", this.ForwardWaitHandle),
                                        new ParameterOverride("temporaryErrorMilliseconds", this.Preferences.ForwardServiceTemporaryErrorBackout),
                                        new ParameterOverride("sleepMilliseconds", Preferences.ForwardServiceMessageBackout)
                                    });
            
            this.Password = password;

            if (password != null)
            {
                this.PersistentStore.Password = password;
            }

            this.EncryptionChecker = container.Resolve<IEncryptionChecker>();            
            if (certificatePolicy != null)
            {
                this.HttpClient.SetCertificatePolicy(certificatePolicy);
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

        internal void ResetStorageSizeLimit()
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
        }

        public void Stop()
        {
            this.NetworkStateService.Stop();
            this.ForwardService.Stop();
            this.StoreService.Stop();
        }

        public void Put(IMessage message)
        {
            this.VolatileStore.Add(message);
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
