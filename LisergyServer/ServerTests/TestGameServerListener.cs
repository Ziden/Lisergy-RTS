using Game.Events.ClientEvents;
using Game.Events.ServerEvents;
using GameServer;
using NUnit.Framework;
using ServerTests;
using System.Linq;

namespace Tests
{
    public class TestGameServerListener
    {
        private TestGame Game;

        [SetUp]
        public void Setup()
        {
            Game = new TestGame();
            Game.NetworkEvents.RegisterListener(new GameServerListener(Game));
        }

        [Test]
        public void TestRoutingInfiniteDungeonPacketsToPlayers()
        {
            var pl = Game.GetTestPlayer();
            Game.HandleClientEvent(pl, new EnterInfiniteDungeonPacket()
            {
                UnitIds = new string[] { pl.Units.First().Id }
            });

            Assert.AreEqual(1, pl.ReceivedEventsOfType<InfiniteDungeonBattlePacket>().Count);
        }
    }
}
