using Game.Events.ServerEvents;
using NUnit.Framework;
using ServerTests.Integration.Stubs;
using System.Threading.Tasks;

namespace ServerTests.Integration
{
    public class TestAuthentication
    {
        MultithreadServers _server;
        TestGameClient _p1;
        TestGameClient _p2;

        [SetUp]
        public void Setup()
        {
            _server = new MultithreadServers();
            _p1 = new TestGameClient();
            _p1.PrepareSDK();
            _p2 = new TestGameClient();
            _p2.PrepareSDK();
        }

        [TearDown]
        public void TearDown() => _server?.Dispose();

        [Test]

        public async Task SmokeTest2p()
        {
            _p1.Modules.Account.SendAuthenticationPacket("abc", "def");
            var result = await _p1.WaitFor<LoginResultPacket>();
            Assert.AreEqual(true, result.Success);

            var p1Received = await _p1.WaitFor<TilePacket>();

            Assert.NotNull(p1Received);
        }
    }
}
