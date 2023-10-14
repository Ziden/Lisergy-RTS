using Game.Events.ServerEvents;
using Game.World;
using NUnit.Framework;
using ServerTests.Integration.Stubs;
using System.Linq;
using System.Threading.Tasks;

namespace ServerTests.Integration
{
    public class SanityTest
    {
        TestServerThread _server;
        TestGameClient _client;

        [SetUp]
        public void Setup()
        {
            _server = new TestServerThread();
            _client = new TestGameClient();
        }

        [TearDown]
        public void TearDown() => _server.Dispose();

        [Test]  
        public async Task TestLogin()
        {
            // LOGIN
            _client.Modules.Account.SendAuthenticationPacket("abc", "def");
            var result = await _client.WaitFor<AuthResultPacket>();
            Assert.AreEqual(true, result.Success);

            // RECEIVE TILES
            var firstReceived = await _client.WaitFor<TilePacket>();
            Assert.NotNull(firstReceived);
            foreach (var tileUpdate in _client.ReceivedPackets.Where(p => p.GetType() == typeof(TilePacket)))
            {
                var tilePacket = (TilePacket)tileUpdate;
                var tile = _client.Game.World.Map.GetTile(tilePacket.Data.X, tilePacket.Data.Y);
                Assert.AreEqual(tile.X, tilePacket.Data.X);
                Assert.AreEqual(tile.Y, tilePacket.Data.Y);
                Assert.AreEqual(tile.SpecId, tilePacket.Data.TileId);

                Assert.AreEqual(new Position(tile.X >> GameWorld.CHUNK_SIZE_BITSHIFT, tile.Y >> GameWorld.CHUNK_SIZE_BITSHIFT), tile.Chunk.Position);
            }
        }

    }
}
