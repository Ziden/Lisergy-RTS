using BattleService;
using Game;
using Game.Battles;
using Game.Events;
using NUnit.Framework;

namespace ServerTests
{
    public class TestBattleServer
    {
        private BattleServer _server;

        [SetUp]
        public void Setup()
        {
            _server = new BattleServer();
        }

        [Test]
        public void TestRunningBattle()
        {
            _server.NetworkEvents.Call(new BattleStartPacket()
            {
                Attacker = new BattleTeam(null, new Unit()),
                Defender = new BattleTeam(null, new Unit()),
                BattleID = "testbattle"
            });

            Assert.AreEqual(1, _server.Listener.RunningBattles.Count);
        }

        [Test]
        public void TestSendingAction()
        {
            _server.NetworkEvents.Call(new BattleStartPacket()
            {
                Attacker = new BattleTeam(null, new Unit()),
                Defender = new BattleTeam(null, new Unit()),
                BattleID = "testbattle"
            });

            _server.NetworkEvents.Call(new BattleStartPacket()
            {
                Attacker = new BattleTeam(null, new Unit()),
                Defender = new BattleTeam(null, new Unit()),
                BattleID = "testbattle"
            });
        }
    }
}
