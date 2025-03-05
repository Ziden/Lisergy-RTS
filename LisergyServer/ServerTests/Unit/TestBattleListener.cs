using Game.Engine.ECLS;
using Game.Entities;
using Game.Network.ClientPackets;
using Game.Network.ServerPackets;
using Game.Systems.Battle;
using Game.Systems.Battler;
using Game.Systems.Movement;
using Game.Systems.Player;
using Game.World;
using NUnit.Framework;
using ServerTests;
using System.Collections.Generic;
using System.Linq;
using Tests.Unit.Stubs;

namespace GameUnitTests
{
    public class TestBattleListener
    {
        private TestGame _game;
        private List<Location> _path;
        private TestServerPlayer _player;
        private IEntity _party;
        private IEntity _dungeon;

        [SetUp]
        public void Setup()
        {
            _game = new TestGame();
            _player = _game.GetTestPlayer();
            _path = new List<Location>();
            _party = _player.Parties[0];
            _dungeon = _game.Entities.CreateEntity(EntityType.Dungeon);
            _dungeon.Logic.Dungeon.SetUnitsFromSpec(_game.Specs.Dungeons[0]);
            _dungeon.Logic.Map.SetPosition(_game.World.GetTile(8, 8));
        }

        private TurnBattle SetupBattle()
        {
            var party = _player.GetParty(0);
            var partyTile = party.GetTile();
            var dungeonTile = partyTile.GetNeighbor(Direction.EAST);
            _dungeon.Logic.Map.SetPosition(dungeonTile);
            var component = party.Get<BattleGroupComponent>();
            var unit = component.Units[0];
            unit.Atk = 255;
            component.Units[0] = unit;
            party.Save(component);
            _player.SendMoveRequest(party, dungeonTile, CourseIntent.OffensiveTarget);

            var course = party.Logic.Movement.GetCourseTask();
            Assert.AreEqual(0, _player.PlayerData.BattleHeaders.Count());
            course.Tick();
            course.Tick();
            return _game.BattleService.GetRunningBattle(party.Get<BattleGroupComponent>().BattleID);
        }

        [Test]
        public void TestBattleTrack()
        {
            var battle = SetupBattle();
            _game.BattleService.BattleTasks[battle.ID].Tick();

            var attackerPlayer = _game.World.Players.GetPlayer(battle.Attacker.OwnerID);
            _game.BattleService.AllBattles.TryGetValue(battle.ID, out var log);


            Assert.That(attackerPlayer.Components.Get<PlayerDataComponent>().BattleHeaders.Any(b => b.BattleID == battle.ID));
            Assert.That(log.Turns.Count() == battle.Record.Turns.Count());
        }

        [Test]
        public void TestRequestinBattleLog()
        {
            var battle = SetupBattle();
            _game.BattleService.BattleTasks[battle.ID].Tick();

            _game.HandleClientEvent(_player, new BattleLogRequestPacket() { BattleId = battle.ID });

            var result = _player.ReceivedPacketsOfType<BattleLogPacket>().FirstOrDefault();

            Assert.NotNull(result);
            Assert.AreEqual(result.Turns.Count(), battle.Record.Turns.Count());
        }
    }
}