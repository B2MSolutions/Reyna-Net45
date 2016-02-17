namespace Reyna.Facts
{
    using Moq;
    using Interfaces;
    using Xunit;
    using System.Text;
    using System.Linq;

    public class GivenAReynaService
    {
        private TestUnityHelper unity = new TestUnityHelper();
        private ReynaService service;

        private Mock<IHttpClient> httpClient;
        private Mock<IRepository> volatileStore;
        private Mock<IRepository> persistentStore;
        private Mock<IPreferences> preferences;
        private Mock<IStoreService> storeService; 
        private Mock<IForwardService> forwardService; 
        private Mock<INetworkStateService> networkStateService;
        private Mock<IEncryptionChecker> encryptionChecker;
        private Mock<IReynaLogger> reynaLogger;

        public GivenAReynaService()
        {
            service = new ReynaService(null, unity.GetContainer());
            httpClient = unity.mockHttpClient;
            preferences = unity.mockPreferences;
            persistentStore = unity.mockSqlStore;
            volatileStore = unity.mockVolatileStore;
            networkStateService = unity.mockNetworkStateService;
            storeService = unity.mockStoreService;
            forwardService = unity.mockForwardService;
            encryptionChecker = unity.mockEncryptionChecker;
            reynaLogger = unity.mockReynaLogger;
        }

        [Fact]
        public void WhenConstructingShouldRegisterCorrectObjectInstancesFromUnity()
        {
            Assert.Same(httpClient.Object, service.HttpClient);
            Assert.Same(preferences.Object, service.Preferences);
            Assert.Same(volatileStore.Object, service.VolatileStore);
            Assert.Same(persistentStore.Object, service.PersistentStore);
            Assert.Same(networkStateService.Object, service.NetworkStateService);
            Assert.Same(forwardService.Object, service.ForwardService);
            Assert.Same(encryptionChecker.Object, service.EncryptionChecker);
            Assert.Same(reynaLogger.Object, service.Logger);
        }

        [Fact]
        public void WhenConstructingShouldCallInitialiseOnStoreService()
        {
            var helper = new TestUnityHelper();
            var container = helper.GetContainer();
            ReynaService service = new ReynaService(null, container);
            helper.mockStoreService.Verify(s => s.Initialize(helper.mockVolatileStore.Object, helper.mockSqlStore.Object), Times.Exactly(1));
        }

        [Fact]
        public void WhenConstructingShouldCallInitialiseOnForwardService()
        {
            var helper = new TestUnityHelper();
            var container = helper.GetContainer();
            ReynaService service = new ReynaService(null, container);
            helper.mockForwardService.Verify(s => s.Initialize(helper.mockSqlStore.Object, helper.mockHttpClient.Object, helper.mockNetworkStateService.Object, It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>()), Times.Exactly(1));
        }

        [Theory]
        [InlineData(42)]
        [InlineData(65)]
        [InlineData(987621)]
        public void WhenCallingGetStorageSizeLimitShouldReturnValueFromPreferences(long limit)
        {
            preferences.SetupGet(p => p.StorageSizeLimit).Returns(limit);
            Assert.Equal(limit, service.StorageSizeLimit);
        }

        [Fact]
        public void WhenCallingResetStorageSizeLimitShouldCallPreferences()
        {
            service.ResetStorageSizeLimit();
            preferences.Verify(p => p.ResetStorageSizeLimit(), Times.Exactly(1));
        }

        [Fact]
        public void WhenCallingSetCellularDataBlackoutShouldCallPreferences()
        {
            var range = new TimeRange(new Time(300), new Time(500));
            service.SetCellularDataBlackout(range);
            preferences.Verify(p => p.SetCellularDataBlackout(range), Times.Exactly(1));
        }

        [Fact]
        public void WhenCallingResetCellularDataBlackoutShouldCallPreferences()
        {
            service.ResetCellularDataBlackout();
            preferences.Verify(p => p.ResetCellularDataBlackout(), Times.Exactly(1));
        }

        [Fact]
        public void WhenCallingSetWlanBlackoutRangeShouldCallPreferences()
        {
            string range = "range";
            service.SetWlanBlackoutRange(range);
            preferences.Verify(p => p.SetWlanBlackoutRange(range), Times.Exactly(1));
        }

        [Fact]
        public void WhenCallingSetWwanBlackoutRangeShouldCallPreferences()
        {
            string range = "range";
            service.SetWwanBlackoutRange(range);
            preferences.Verify(p => p.SetWwanBlackoutRange(range), Times.Exactly(1));
        }

        [Fact]
        public void WhenCallingSetRoamingBlackoutShouldCallPreferences()
        {
            bool blackout = false;
            service.SetRoamingBlackout(blackout);
            preferences.Verify(p => p.SetRoamingBlackout(blackout), Times.Exactly(1));
        }

        [Fact]
        public void WhenCallingSetOnChargeBlackoutShouldCallPreferences()
        {
            bool blackout = true;
            service.SetOnChargeBlackout(blackout);
            preferences.Verify(p => p.SetOnChargeBlackout(blackout), Times.Exactly(1));
        }

        [Fact]
        public void WhenCallingSetOffChargeBlackoutShouldCallPreferences()
        {
            bool blackout = false;
            service.SetOffChargeBlackout(blackout);
            preferences.Verify(p => p.SetOffChargeBlackout(blackout), Times.Exactly(1));
        }

        [Fact]
        public void WhenCallingSetStorageSizeLimitShouldInitialiseThePersistentStoreAndShrinkTheDatabase()
        {
            long limit = 2867776;
            service.SetStorageSizeLimit(limit);
            persistentStore.Verify(s => s.Initialise(), Times.Exactly(1));
            persistentStore.Verify(s => s.ShrinkDb(limit), Times.Exactly(1));
        }

        [Fact]
        public void WhenCallingSetPersistentStoreAndStorageSizeLimitBelowMinimumShouldSetTheLimitToMinimum()
        {
            long limit = 186777;
            service.SetStorageSizeLimit(limit);
            persistentStore.Verify(s => s.Initialise(), Times.Exactly(1));
            persistentStore.Verify(s => s.ShrinkDb(ReynaService.MinimumStorageLimit), Times.Exactly(1));
        }

        [Fact]
        public void WhenCallingSetStorageSizeShouldCallPreferences()
        {
            long limit = 2186777;
            service.SetStorageSizeLimit(limit);
            preferences.Verify(p => p.SetStorageSizeLimit(limit), Times.Exactly(1));
        }

        [Fact]
        public void WhenCallingStartShouldStartAllServices()
        {
            service.Start();
            storeService.Verify(s => s.Start(), Times.Exactly(1));
            forwardService.Verify(s => s.Start(), Times.Exactly(1));
            networkStateService.Verify(s => s.Start(), Times.Exactly(1));
        }

        [Fact]
        public void WhenCallingStopShouldStopAllServices()
        {
            service.Stop();
            storeService.Verify(s => s.Stop(), Times.Exactly(1));
            forwardService.Verify(s => s.Stop(), Times.Exactly(1));
            networkStateService.Verify(s => s.Stop(), Times.Exactly(1));
        }

        [Fact]
        public void WhenCallingDisposeShouldDisposeAllServices()
        {
            service.Dispose();
            storeService.Verify(s => s.Dispose(), Times.Exactly(1));
            forwardService.Verify(s => s.Dispose(), Times.Exactly(1));
            networkStateService.Verify(s => s.Dispose(), Times.Exactly(1));
        }

        [Fact]
        public void WhenCallingDisposeAndServicesAreNullShouldNotDisposeServices()
        {
            var helper = new TestUnityHelper();
            var container = helper.GetContainer();
            ReynaService service = new ReynaService(null, container);
            service.StoreService = null;
            service.ForwardService = null;
            service.NetworkStateService = null;
            service.Dispose();
            helper.mockStoreService.Verify(s => s.Dispose(), Times.Never);
            helper.mockForwardService.Verify(s => s.Dispose(), Times.Never);
            helper.mockNetworkStateService.Verify(s => s.Dispose(), Times.Never);
        }

        [Fact]
        public void WhenConstructingWithAPasswordPasswordFieldShouldBeSet()
        {
            byte[] bytes = Encoding.ASCII.GetBytes("password");
            var helper = new TestUnityHelper();
            var container = helper.GetContainer();
            ReynaService service = new ReynaService(bytes, container);
            Assert.True(bytes.SequenceEqual(service.Password));
        }

        [Fact]
        public void WhenCallingStartAndPasswordIsSetAndDbIsNotEncryptedShouldEncryptDbUsingPassword()
        {
            var helper = new TestUnityHelper();
            var container = helper.GetContainer();
            byte[] bytes = Encoding.ASCII.GetBytes("password");
            ReynaService service = new ReynaService(bytes, container);
            helper.mockEncryptionChecker.Setup(m => m.DbEncrypted()).Returns(false);

            service.Start();

            helper.mockEncryptionChecker.Verify(m => m.EncryptDb(bytes), Times.Exactly(1));
        }

        [Fact]
        public void WhenCallingStartAndPasswordIsSetAndDbIsEncryptedShouldNotEncryptDb()
        {
            var helper = new TestUnityHelper();
            var container = helper.GetContainer();
            byte[] bytes = Encoding.ASCII.GetBytes("password");
            ReynaService service = new ReynaService(bytes, container);
            helper.mockEncryptionChecker.Setup(m => m.DbEncrypted()).Returns(true);

            service.Start();

            helper.mockEncryptionChecker.Verify(m => m.EncryptDb(bytes), Times.Never);
        }

        [Fact]
        public void WhenCallingStartAndPasswordIsNotSetAndShouldNotEncryptDb()
        {
            var helper = new TestUnityHelper();
            var container = helper.GetContainer();
            ReynaService service = new ReynaService(null, container);

            service.Start();

            helper.mockEncryptionChecker.Verify(m => m.DbEncrypted(), Times.Never);
        }

        [Fact]
        public void WhenCallingPutShouldCallVolatileStore()
        {
            IMessage message = new Message(new System.Uri("http://testuri.com"), "body");
            service.Put(message);
            volatileStore.Verify(s => s.Add(message), Times.Exactly(1));
        }

        [Fact]
        public void WhenConstructingWithAPasswordShouldSetPasswordOnSecureStore()
        {
            var helper = new TestUnityHelper();
            var container = helper.GetContainer();
            byte[] bytes = Encoding.ASCII.GetBytes("password");

            helper.mockSqlStore.SetupSet(s => s.Password = bytes).Verifiable();

            ReynaService service = new ReynaService(bytes, container);

            helper.mockSqlStore.VerifySet(s => s.Password=bytes, Times.Exactly(1));
        }

        [Fact]
        public void WhenConstructingWithoutAPasswordShouldnotSetPasswordOnSecureStore()
        {
            var helper = new TestUnityHelper();
            var container = helper.GetContainer();

            helper.mockSqlStore.SetupSet(s => s.Password = It.IsAny<byte[]>()).Verifiable();

            ReynaService service = new ReynaService(null, container);

            helper.mockSqlStore.VerifySet(s => s.Password = It.IsAny<byte[]>(), Times.Never);
        }
    }
}
