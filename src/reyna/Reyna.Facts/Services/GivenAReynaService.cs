namespace Reyna.Facts
{
    using System.IO;
    using Microsoft.Win32;
    using Moq;
    using Reyna.Interfaces;
    using Xunit;
    using Xunit.Extensions;
    using Microsoft.Practices.Unity;
    using System.Net;

    public class GivenAReynaService
    {
        private TestUnityHelper unity = new TestUnityHelper();
        private ReynaService service;

        private Mock<IHttpClient> httpClient;
        private Mock<IRepository> volatileStore;
        private Mock<IRepository> persistentStore;
        private Mock<IPreferences> preferences;
        private Mock<IService> storeService; 
        private Mock<IService> forwardService; 
        private Mock<ISystemNotifier> systemNotifier;
        private Mock<IWaitHandle> networkWaitHandle;
        private Mock<INetworkStateService> networkStateService;
        private Mock<IWaitHandle> forwardWaitHandle;
        private Mock<IWaitHandle> storeWaitHandle;

        public GivenAReynaService()
        {
            this.service = new ReynaService(null, null, this.unity.GetContainer());
            this.httpClient = this.unity.mockHttpClient;
            this.preferences = this.unity.mockPreferences;
            this.persistentStore = this.unity.mockSqlStore;
            this.volatileStore = this.unity.mockVolatileStore;
            this.systemNotifier = this.unity.mockSystemNotifier;
            this.networkWaitHandle = this.unity.mockNetworkWaitHandle;
            this.networkStateService = this.unity.mockNetworkStateService;
            this.forwardWaitHandle = this.unity.mockForwardWaitHandle;
            this.storeWaitHandle = this.unity.mockStoreWaitHandle;
            this.storeService = this.unity.mockStoreService;
            this.forwardService = this.unity.mockForwardService;
        }

        [Fact]
        public void WhenConstructingShouldRegisterCorrectObjectInstancesFromUnity()
        {
            Assert.Same(this.httpClient.Object, this.service.HttpClient);
            Assert.Same(this.preferences.Object, this.service.Preferences);
            Assert.Same(this.volatileStore.Object, this.service.VolatileStore);
            Assert.Same(this.persistentStore.Object, this.service.PersistentStore);
            Assert.Same(this.systemNotifier.Object, this.service.SystemNotifier);
            Assert.Same(this.networkWaitHandle.Object, this.service.NetworkWaitHandle);
            Assert.Same(this.networkStateService.Object, this.service.NetworkStateService);
            Assert.Same(this.forwardWaitHandle.Object, this.service.ForwardWaitHandle);
            Assert.Same(this.storeWaitHandle.Object, this.service.StoreWaitHandle);
            Assert.Same(this.storeService.Object, this.service.StoreService);
            Assert.Same(this.forwardService.Object, this.service.ForwardService);
        }

        [Theory]
        [InlineData(42)]
        [InlineData(65)]
        [InlineData(987621)]
        public void WhenCallingGetStorageSizeLimitShouldReturnValueFromPreferences(long limit)
        {
            this.preferences.SetupGet(p => p.StorageSizeLimit).Returns(limit);
            Assert.Equal(limit, this.service.StorageSizeLimit);
        }

        [Fact]
        public void WhenCallingResetStorageSizeLimitShouldCallPreferences()
        {
            this.service.ResetStorageSizeLimit();
            this.preferences.Verify(p => p.ResetStorageSizeLimit(), Times.Exactly(1));
        }

        [Fact]
        public void WhenCallingSetCellularDataBlackoutShouldCallPreferences()
        {
            var range = new TimeRange(new Time(300), new Time(500));
            this.service.SetCellularDataBlackout(range);
            this.preferences.Verify(p => p.SetCellularDataBlackout(range), Times.Exactly(1));
        }

        [Fact]
        public void WhenCallingResetCellularDataBlackoutShouldCallPreferences()
        {
            this.service.ResetCellularDataBlackout();
            this.preferences.Verify(p => p.ResetCellularDataBlackout(), Times.Exactly(1));
        }

        [Fact]
        public void WhenCallingSetWlanBlackoutRangeShouldCallPreferences()
        {
            string range = "range";
            this.service.SetWlanBlackoutRange(range);
            this.preferences.Verify(p => p.SetWlanBlackoutRange(range), Times.Exactly(1));
        }

        [Fact]
        public void WhenCallingSetWwanBlackoutRangeShouldCallPreferences()
        {
            string range = "range";
            this.service.SetWwanBlackoutRange(range);
            this.preferences.Verify(p => p.SetWwanBlackoutRange(range), Times.Exactly(1));
        }

        [Fact]
        public void WhenCallingSetRoamingBlackoutShouldCallPreferences()
        {
            bool blackout = false;
            this.service.SetRoamingBlackout(blackout);
            this.preferences.Verify(p => p.SetRoamingBlackout(blackout), Times.Exactly(1));
        }

        [Fact]
        public void WhenCallingSetOnChargeBlackoutShouldCallPreferences()
        {
            bool blackout = true;
            this.service.SetOnChargeBlackout(blackout);
            this.preferences.Verify(p => p.SetOnChargeBlackout(blackout), Times.Exactly(1));
        }

        [Fact]
        public void WhenCallingSetOffChargeBlackoutShouldCallPreferences()
        {
            bool blackout = false;
            this.service.SetOffChargeBlackout(blackout);
            this.preferences.Verify(p => p.SetOffChargeBlackout(blackout), Times.Exactly(1));
        }

        [Fact]
        public void WhenCallingSetStorageSizeLimitShouldInitialiseThePersistentStoreAndShrinkTheDatabase()
        {
            long limit = 2867776;
            this.service.SetStorageSizeLimit(limit);
            this.persistentStore.Verify(s => s.Initialise(), Times.Exactly(1));
            this.persistentStore.Verify(s => s.ShrinkDb(limit), Times.Exactly(1));
        }

        [Fact]
        public void WhenCallingSetPersistentStoreAndStorageSizeLimitBelowMinimumShouldSetTheLimitToMinimum()
        {
            long limit = 186777;
            this.service.SetStorageSizeLimit(limit);
            this.persistentStore.Verify(s => s.Initialise(), Times.Exactly(1));
            this.persistentStore.Verify(s => s.ShrinkDb(ReynaService.MinimumStorageLimit), Times.Exactly(1));
        }

        [Fact]
        public void WhenCallingSetStorageSizeShouldCallPreferences()
        {
            long limit = 2186777;
            this.service.SetStorageSizeLimit(limit);
            this.preferences.Verify(p => p.SetStorageSizeLimit(limit), Times.Exactly(1));
        }

        [Fact]
        public void WhenCreatingAReynaServiceWithACertificatePolicyShouldSetCertificatePolicyOnHttpClient()
        {
            var helper = new TestUnityHelper();
            var container = helper.GetContainer();
            Mock<ICertificatePolicy> policy = new Mock<ICertificatePolicy>();
            ReynaService service = new ReynaService(null, policy.Object, container);
            helper.mockHttpClient.Verify(h => h.SetCertificatePolicy(policy.Object), Times.Exactly(1));
        }

        [Fact]
        public void WhenCreatingAReynaServiceWithNoCertificatePolicyShouldNotSetCertificatePolicyOnHttpClient()
        {
            var helper = new TestUnityHelper();
            var container = helper.GetContainer();
            ReynaService service = new ReynaService(null, null, container);
            helper.mockHttpClient.Verify(h => h.SetCertificatePolicy(It.IsAny<ICertificatePolicy>()), Times.Never);
        }

        [Fact]
        public void WhenCallingStartShouldStartAllServices()
        {
            this.service.Start();
            this.storeService.Verify(s => s.Start(), Times.Exactly(1));
            this.forwardService.Verify(s => s.Start(), Times.Exactly(1));
            this.networkStateService.Verify(s => s.Start(), Times.Exactly(1));
        }

        [Fact]
        public void WhenCallingStopShouldStopAllServices()
        {
            this.service.Stop();
            this.storeService.Verify(s => s.Stop(), Times.Exactly(1));
            this.forwardService.Verify(s => s.Stop(), Times.Exactly(1));
            this.networkStateService.Verify(s => s.Stop(), Times.Exactly(1));
        }

        [Fact]
        public void WhenCallingDisposeShouldDisposeAllServices()
        {
            this.service.Dispose();
            this.storeService.Verify(s => s.Dispose(), Times.Exactly(1));
            this.forwardService.Verify(s => s.Dispose(), Times.Exactly(1));
            this.networkStateService.Verify(s => s.Dispose(), Times.Exactly(1));
        }

        [Fact]
        public void WhenCallingDisposeAndServicesAreNullShouldNotDisposeServices()
        {
            var helper = new TestUnityHelper();
            var container = helper.GetContainer();
            ReynaService service = new ReynaService(null, null, container);
            service.StoreService = null;
            service.ForwardService = null;
            service.NetworkStateService = null;
            service.Dispose();
            helper.mockStoreService.Verify(s => s.Dispose(), Times.Never);
            helper.mockForwardService.Verify(s => s.Dispose(), Times.Never);
            helper.mockNetworkStateService.Verify(s => s.Dispose(), Times.Never);
        }

        //[Fact]
        //public void WhenConstructingAndReceivedPasswordShouldPassPasswordToSQLiteRepository()
        //{
        //    var password = new byte[] { 0xFF, 0xAA, 0xCC, 0xCC };
        //    var reynaService = new ReynaService(password, null);

        //    Assert.Equal(password, ((SQLiteRepository)reynaService.PersistentStore).Password); 
        //}

        //[Fact]
        //public void WhenConstructingWithoutPasswordShouldNotPassAnyPasswordToSQLiteRepository()
        //{
        //    var reynaService = new ReynaService();

        //    Assert.Null(((SQLiteRepository)reynaService.PersistentStore).Password);
        //}

        //[Fact]
        //public void WhenCallingPutShouldAddMessage()
        //{
        //    this.VolatileStore.Setup(r => r.Add(It.IsAny<IMessage>()));

        //    var message = new Message(null, null);
        //    this.ReynaService.Put(message);

        //    this.VolatileStore.Verify(r => r.Add(message), Times.Once());
        //}

        //[Fact]
        //public void WhenCallingStartShouldStartStoreService()
        //{
        //    this.StoreService.Setup(s => s.Start());
        //    this.ForwardService.Setup(f => f.Start());
        //    this.NetworkStateService.Setup(f => f.Start());

        //    this.ReynaService.Start();

        //    this.StoreService.Verify(s => s.Start(), Times.Once());
        //    this.ForwardService.Verify(f => f.Start(), Times.Once());
        //    this.NetworkStateService.Verify(f => f.Start(), Times.Once());
        //}

        //[Fact]
        //public void WhenCallingStopShouldStopStoreService()
        //{
        //    this.StoreService.Setup(s => s.Stop());
        //    this.ForwardService.Setup(f => f.Stop());
        //    this.NetworkStateService.Setup(f => f.Stop());

        //    this.ReynaService.Stop();

        //    this.StoreService.Verify(s => s.Stop(), Times.Once());
        //    this.ForwardService.Verify(f => f.Stop(), Times.Once());
        //    this.NetworkStateService.Verify(f => f.Stop(), Times.Once());
        //}

        //[Fact]
        //public void WhenCallingDisposeShouldCallDisposeOnStoreService()
        //{
        //    this.StoreService.Setup(s => s.Dispose());
        //    this.ForwardService.Setup(s => s.Dispose());
        //    this.NetworkStateService.Setup(s => s.Dispose());

        //    this.ReynaService.Dispose();

        //    this.StoreService.Verify(s => s.Dispose(), Times.Once());
        //    this.ForwardService.Verify(f => f.Dispose(), Times.Once());
        //    this.NetworkStateService.Verify(f => f.Dispose(), Times.Once());
        //}

        //[Fact]
        //public void WhenGettingForwardServiceTemporaryErrorBackoutAndNoRegistryKeyShouldReturnDefault5Minutes()
        //{
        //    Assert.Equal(300000, Preferences.ForwardServiceTemporaryErrorBackout);
        //}

        //[Fact]
        //public void WhenGettingForwardServiceTemporaryErrorBackoutAndRegistryKeyExistsShouldReturnExpected()
        //{
        //    using (var key = Registry.LocalMachine.CreateSubKey(@"Software\Reyna"))
        //    {
        //        key.SetValue("TemporaryErrorBackout", 100);
        //    }

        //    Assert.Equal(100, Preferences.ForwardServiceTemporaryErrorBackout);

        //    Registry.LocalMachine.DeleteSubKey(@"Software\Reyna", false);
        //}

        //[Fact]
        //public void WhenGettingForwardServiceMessageBackoutAndNoRegistryKeyShouldReturnDefault5Minutes()
        //{
        //    Assert.Equal(1000, Preferences.ForwardServiceMessageBackout);
        //}

        //[Fact]
        //public void WhenGettingForwardServiceMessageBackoutAndRegistryKeyExistsShouldReturnExpected()
        //{
        //    using (var key = Registry.LocalMachine.CreateSubKey(@"Software\Reyna"))
        //    {
        //        key.SetValue("MessageBackout", 10);
        //    }

        //    Assert.Equal(10, Preferences.ForwardServiceMessageBackout);

        //    Registry.LocalMachine.DeleteSubKey(@"Software\Reyna", false);
        //}

        //[Fact]
        //public void WhenSettingStorageLimitShouldSaveStorageLimit()
        //{
        //    ReynaService.SetStorageSizeLimit(null, 3145728);
        //    Assert.Equal(3145728, ReynaService.StorageSizeLimit);

        //    Registry.LocalMachine.DeleteSubKey(@"Software\Reyna", false);
        //}

        //[Fact]
        //public void WhenGettingStorageLimitShouldSaveStorageLimit()
        //{
        //    ReynaService.SetStorageSizeLimit(null, 3145728);
        //    Assert.Equal(3145728, ReynaService.StorageSizeLimit);

        //    Registry.LocalMachine.DeleteSubKey(@"Software\Reyna", false);
        //}

        //[Fact]
        //public void WhenSettingStorageLimitShouldInitializeReyna()
        //{
        //    File.Delete("reyna.db");
        //    ReynaService.SetStorageSizeLimit(null, 3145728);
        //    Assert.True(File.Exists("reyna.db"));
        //}
        
        //[Theory]
        //[InlineData(-42)]
        //[InlineData(0)]
        //[InlineData(42)]
        //public void WhenSettingStorageLimitShouldSetToMinimumValue(long value) 
        //{
        //    ReynaService.SetStorageSizeLimit(null, value);
        //    Assert.Equal(1867776, ReynaService.StorageSizeLimit); // 1867776 - min value, 1.8 Mb

        //    Registry.LocalMachine.DeleteSubKey(@"Software\Reyna", false);
        //}
        
        //[Fact]
        //public void WhenResettingsStorageLimitShouldDeleteIt()
        //{
        //    ReynaService.SetStorageSizeLimit(null, 100);
        //    ReynaService.ResetStorageSizeLimit();
        //    Assert.Equal(-1, ReynaService.StorageSizeLimit);
        //}

        //[Fact]
        //public void WhenResettingsStorageLimitAndRegistryKeyNotExistsShouldNotThrows()
        //{
        //    Registry.LocalMachine.DeleteSubKey(@"Software\Reyna", false);
            
        //    ReynaService.ResetStorageSizeLimit();
        //}

        //[Fact]
        //public void WhenSetCellularDataBlackoutShouldStoreIt()
        //{
        //    TimeRange range = new TimeRange(new Time(11, 00), new Time(12, 01));
        //    ReynaService.SetCellularDataBlackout(range);

        //    TimeRange timeRange = new Preferences().CellularDataBlackout;

        //    Assert.Equal(range.From.MinuteOfDay, timeRange.From.MinuteOfDay);
        //    Assert.Equal(range.To.MinuteOfDay, timeRange.To.MinuteOfDay);
        //}

        //[Fact]
        //public void WhenResetCellularDataBlackoutThenGetCellularDataBlackoutShouldReturnNull()
        //{
        //    TimeRange range = new TimeRange(new Time(11, 00), new Time(12, 01));
        //    ReynaService.ResetCellularDataBlackout();

        //    TimeRange timeRange = new Preferences().CellularDataBlackout;

        //    Assert.Null(timeRange);
        //}

        //[Fact]
        //public void WhenSetWlanBlackoutRangeShouldStoreIt()
        //{
        //    string range = "00:00-00:10";
        //    ReynaService.SetWlanBlackoutRange(range);

        //    string actual = new Preferences().WlanBlackoutRange;

        //    Assert.Equal(range, actual);
        //}

        //[Fact]
        //public void WhenSetWwanBlackoutRangeShouldStoreIt()
        //{
        //    string range = "00:00-00:10";
        //    ReynaService.SetWwanBlackoutRange(range);

        //    string actual = new Preferences().WwanBlackoutRange;

        //    Assert.Equal(range, actual);
        //}

        //[Fact]
        //public void WhenSetRoamingBlackoutShouldStoreIt()
        //{
        //    ReynaService.SetRoamingBlackout(false);

        //    bool actual = new Preferences().RoamingBlackout;

        //    Assert.False(actual);
        //}

        //[Fact]
        //public void WhenSetOnChargeBlackoutShouldStoreIt()
        //{
        //    ReynaService.SetOnChargeBlackout(false);

        //    bool actual = new Preferences().OnChargeBlackout;

        //    Assert.False(actual);
        //}

        //[Fact]
        //public void WhenSetOffChargeBlackoutShouldStoreIt()
        //{
        //    ReynaService.SetOffChargeBlackout(false);

        //    bool actual = new Preferences().OffChargeBlackout;

        //    Assert.False(actual);
        //}
    }
}
