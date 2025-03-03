using Game.Entities;
using Game.Events.ServerEvents;
using NUnit.Framework;
using ServerTests;
using System.Linq;

namespace UnitTests
{
    public class TestWorldEvents
    {
        private TestGame game;

        [Test]
        public void TestSendingTestPlayerEvents()
        {
            game = new TestGame();
            var player = game.GetTestPlayer();

            var events = game.SentServerPackets
                .Where(e => e is EntityUpdatePacket p && p.Type == EntityType.Tile)
                .Select(e => (EntityUpdatePacket)e)
                .ToList();
        }
    }
}