using Game.Events.ServerEvents;
using NUnit.Framework;
using ServerTests.Integration.Stubs;
using System.Threading.Tasks;
using Telepathy;

namespace ServerTests.Integration
{
    public class TwoPlayerTest
    {
        TestServerThread _server;
        TestGameClient _p1;
        TestGameClient _p2;

        [SetUp]
        public void Setup()
        {
            _server = new TestServerThread();
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

            _p2.Modules.Account.SendAuthenticationPacket("abc2", "def2");
            var resultP2 = await _p2.WaitFor<LoginResultPacket>();
            Assert.AreEqual(true, resultP2.Success);

            var p1Received = await _p1.WaitFor<TilePacket>();
            var p2Received = await _p2.WaitFor<TilePacket>();

            Assert.NotNull(p1Received);
            Assert.NotNull(p2Received);
            Assert.AreNotEqual(p1Received.Data, p2Received.Data);
        }

        [Test]

        public async Task SmokeTestP1Disconnects()
        {
            _p1.Modules.Account.SendAuthenticationPacket("abc", "def");
            var result = await _p1.WaitFor<LoginResultPacket>();
            Assert.AreEqual(true, result.Success);
            _p1.Network.Disconnect();

            _p2.Modules.Account.SendAuthenticationPacket("abc2", "def2");
            var resultP2 = await _p2.WaitFor<LoginResultPacket>();
            Assert.AreEqual(true, resultP2.Success);

            var p1Received = await _p1.WaitFor<TilePacket>();
            var p2Received = await _p2.WaitFor<TilePacket>();

            Assert.IsNull(_server.Server.ServerError);
            Assert.Null(p1Received);
            Assert.NotNull(p2Received);
        }
    }
}
