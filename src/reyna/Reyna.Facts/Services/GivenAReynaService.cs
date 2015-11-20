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
            this.service = new ReynaService(null, this.unity.GetContainer());
            this.httpClient = this.unity.mockHttpClient;
            this.preferences = this.unity.mockPreferences;
            this.persistentStore = this.unity.mockSqlStore;
            this.volatileStore = this.unity.mockVolatileStore;
            this.networkStateService = this.unity.mockNetworkStateService;
            this.storeService = this.unity.mockStoreService;
            this.forwardService = this.unity.mockForwardService;
            this.encryptionChecker = this.unity.mockEncryptionChecker;
            this.reynaLogger = this.unity.mockReynaLogger;
        }

        [Fact]
        public void WhenConstructingShouldRegisterCorrectObjectInstancesFromUnity()
        {
            Assert.Same(this.httpClient.Object, this.service.HttpClient);
            Assert.Same(this.preferences.Object, this.service.Preferences);
            Assert.Same(this.volatileStore.Object, this.service.VolatileStore);
            Assert.Same(this.persistentStore.Object, this.service.PersistentStore);
            Assert.Same(this.networkStateService.Object, this.service.NetworkStateService);
            Assert.Same(this.forwardService.Object, this.service.ForwardService);
            Assert.Same(this.encryptionChecker.Object, this.service.EncryptionChecker);
            Assert.Same(this.reynaLogger.Object, this.service.Logger);
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
            helper.mockForwardService.Verify(s => s.Initialize(helper.mockSqlStore.Object, helper.mockHttpClient.Object, helper.mockNetworkStateService.Object, It.IsAny<int>(), It.IsAny<int>()), Times.Exactly(1));
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
            this.service.Put(message);
            this.volatileStore.Verify(s => s.Add(message), Times.Exactly(1));
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
