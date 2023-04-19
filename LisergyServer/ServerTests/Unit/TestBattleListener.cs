using Game.Battle;
using Game.Battler;
using Game.Dungeon;
using Game.Events.ServerEvents;
using Game.Movement;
using Game.Network;
using Game.Network.ClientPackets;
using Game.Network.ServerPackets;
using Game.Party;
using Game.Pathfinder;
using Game.Scheduler;
using Game.Tile;
using Game.World;
using NUnit.Framework;
using ServerTests;
using System.Collections.Generic;
using System.Linq;

namespace Tests
{
    public class TestBattleListener
    {
        private TestGame _game;
        private List<MapPosition> _path;
        private TestServerPlayer _player;
        private PartyEntity _party;
        private DungeonEntity _dungeon;

        [SetUp]
        public void Setup()
        {
            _game = new TestGame();
            _player = _game.GetTestPlayer();
            _path = new List<MapPosition>();
            _party = _player.GetParty(0);
            _dungeon = new DungeonEntity(0);
            _dungeon.Tile = _game.World.GetTile(8, 8);
            GameScheduler.Clear();
        }

        [TearDown]
        public void Tear()
        {
            _game.ClearEventListeners();
        }
    

        private TurnBattle SetupBattle()
        {
            var party = _player.GetParty(0);
            var partyTile = party.Tile;
            var dungeonTile = partyTile.GetNeighbor(Direction.EAST);
            _dungeon.Tile = dungeonTile;
            party.BattleGroupLogic.GetUnits().First().Atk = 255;
            _player.SendMoveRequest(party, dungeonTile, MovementIntent.Offensive);
            var course = party.Course;
            Assert.AreEqual(0, _player.BattleHeaders.Count());
            course.Execute();
            course.Execute();
            return _game.BattleService.GetBattle(party.BattleGroupLogic.BattleID);
        }

        [Test]
        public void TestBattleTrack()
        {
            var battle = SetupBattle();
            battle.Task.Execute();

            var attackerPlayer = _game.World.Players.GetPlayer(battle.Attacker.OwnerID);
            BattleHistory.TryGetLog(battle.ID, out var log);


            Assert.That(attackerPlayer.BattleHeaders.ContainsKey(battle.ID));
            Assert.That(log.Turns.Count() == battle.Result.Turns.Count());
        }

        [Test]
        public void TestRequestinBattleLog()
        {
            var battle = SetupBattle();
            battle.Task.Execute();

            _game.HandleClientEvent(_player, new BattleLogRequestPacket() { BattleId = battle.ID });

            var result = _player.ReceivedEventsOfType<BattleLogPacket>().FirstOrDefault();

            Assert.NotNull(result);
            Assert.AreEqual(result.Turns.Count(), battle.Result.Turns.Count());
        }
    }
}