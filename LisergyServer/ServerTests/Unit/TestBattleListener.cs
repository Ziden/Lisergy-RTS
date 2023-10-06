using Game;
using Game.Battle;
using Game.Network.ClientPackets;
using Game.Network.ServerPackets;
using Game.Scheduler;
using Game.Systems.Battler;
using Game.Systems.Dungeon;
using Game.Systems.Movement;
using Game.Systems.Party;
using Game.Systems.Tile;
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
        private List<Position> _path;
        private TestServerPlayer _player;
        private PartyEntity _party;
        private DungeonEntity _dungeon;

        [SetUp]
        public void Setup()
        {
            _game = new TestGame();
            _player = _game.GetTestPlayer();
            _path = new List<Position>();
            _party = _player.GetParty(0);
            _dungeon = _game.Entities.CreateEntity<DungeonEntity>(null);
            _dungeon.BuildFromSpec(_game.Specs.Dungeons[0]);
            _game.Systems.Map.GetEntityLogic(_dungeon).SetPosition(_game.World.GetTile(8, 8));
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
            var partyTile = _game.Systems.Map.GetEntityLogic(party).GetPosition();
            var dungeonTile = partyTile.GetNeighbor(Direction.EAST);
            _game.Systems.Map.GetEntityLogic(_dungeon).SetPosition(dungeonTile);
            party.Get<BattleGroupComponent>().Units.First().Atk = 255;
            _player.SendMoveRequest(party, dungeonTile, MovementIntent.Offensive);
            var course = party.Course;
            Assert.AreEqual(0, _player.Data.BattleHeaders.Count());
            course.Tick();
            course.Tick();
            return _game.BattleService.GetBattle(party.Get<BattleGroupComponent>().BattleID);
        }

        [Test]
        public void TestBattleTrack()
        {
            var battle = SetupBattle();
            battle.Task.Tick();

            var attackerPlayer = _game.World.Players.GetPlayer(battle.Attacker.OwnerID);
            BattleHistory.TryGetLog(battle.ID, out var log);


            Assert.That(attackerPlayer.Data.BattleHeaders.ContainsKey(battle.ID));
            Assert.That(log.Turns.Count() == battle.Result.Turns.Count());
        }

        [Test]
        public void TestRequestinBattleLog()
        {
            var battle = SetupBattle();
            battle.Task.Tick();

            _game.HandleClientEvent(_player, new BattleLogRequestPacket() { BattleId = battle.ID });

            var result = _player.ReceivedEventsOfType<BattleLogPacket>().FirstOrDefault();

            Assert.NotNull(result);
            Assert.AreEqual(result.Turns.Count(), battle.Result.Turns.Count());
        }
    }
}