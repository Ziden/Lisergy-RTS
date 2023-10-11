using ClientSDK;
using Game.Events.ServerEvents;
using Game.Network.ClientPackets;
using NUnit.Framework;
using ServerTests.Integration.Stubs;
using System.Threading.Tasks;

namespace ServerTests.Integration
{
    public class BaseSDKTests
    {
        TestServerThread _server;

        [SetUp]
        public void Setup()
        {
            _server = new TestServerThread();
        }

        [TearDown]
        public void TearDown() => _server.Dispose();

        [Test]  
        public async Task TestLogin()
        {
            var client = new TestGameClient();
            client.Services.Account.SendAuthenticationPacket("abc", "def");
            var result = await client.WaitFor<AuthResultPacket>();



            Assert.AreEqual(true, result.Success);
        }
    }
}
