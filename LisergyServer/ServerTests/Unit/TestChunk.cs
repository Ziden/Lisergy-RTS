using Game;
using Game.Events;
using Game.Events.ServerEvents;
using NUnit.Framework;
using ServerTests;
using System.Linq;

namespace Tests
{
    public class TestWorldEvents
    {
        private TestGame game;

        [Test]
        public void TestSendingTestPlayerEvents()
        {
            game = new TestGame();
            var player = game.GetTestPlayer();

            var events = game.ReceivedEvents
                .Where(e => e is TileVisibleEvent)
                .Select(e => (TileVisibleEvent)e)
                .ToList();
        }
    }
}