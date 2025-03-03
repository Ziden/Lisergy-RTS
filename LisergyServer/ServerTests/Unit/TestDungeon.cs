using Game.Engine.ECLS;
using Game.Entities;
using Game.Events.ServerEvents;
using Game.Network.ServerPackets;
using Game.Systems.Battle;
using Game.Systems.Battle.Data;
using Game.Systems.Battler;
using Game.Systems.Map;
using Game.Systems.Movement;
using Game.Systems.Tile;
using Game.World;
using NUnit.Framework;
using ServerTests;
using System.Collections.Generic;
using System.Linq;
using Tests.Unit.Stubs;

namespace UnitTests
{
    public class TestDungeon
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
            _party = _player.GetParty(0);
            _dungeon = _game.Entities.CreateEntity(EntityType.Dungeon);
            _dungeon.Logic.Dungeon.SetUnitsFromSpec(_game.Specs.Dungeons[0]);

            Assert.That(_dungeon.Get<BattleGroupComponent>().Units.Valids == 1);
            _dungeon.Logic.Map.SetPosition(_game.World.GetTile(8, 8));
        }

        [TearDown]
        public void TearDown()
        {
            // EventPoolValidator.ValidateDisposed();
        }

        [Test]
        public void TestPassiveMoveTowardsDungeon()
        {
            var playerCastleTile = _player.Buildings.First().GetTile();
            var dungeonTile = playerCastleTile.GetNeighbor(Direction.EAST);
            var party = _player.GetParty(0);

            // place the dungeon reference
            dungeonTile.Components.Get<TileHabitantsComponent>().Building = _dungeon;

            // send intent to move player to the party
            _player.SendMoveRequest(party, dungeonTile, CourseIntent.Defensive);
            var course = _player.GetParty(0).Course();

            // Complete the move intent
            _game.GameScheduler.ForceComplete(course);

            Assert.AreEqual(dungeonTile, party.GetTile());
            Assert.IsTrue(party.Components.Get<BattleGroupComponent>().BattleID.IsZero());
            Assert.AreEqual(0, _player.PlayerData.BattleHeaders.Count());
        }

        [Test]
        public void TestAgressiveMoveOnDungeon()
        {
            var party = _player.GetParty(0);
            var partyTile = party.GetTile();
            var dungeonTile = partyTile.GetNeighbor(Direction.EAST);

            _dungeon.Logic.Map.SetPosition(dungeonTile);

            var component = party.Components.Get<BattleGroupComponent>();
            var unit = component.Units[0];
            unit.Atk = 255;
            component.Units[0] = unit;
            party.Save(component);

            _player.SendMoveRequest(party, dungeonTile, CourseIntent.OffensiveTarget);
            var course = party.Course();

            Assert.AreEqual(0, _player.PlayerData.BattleHeaders.Count());

            course.Tick();
            course.Tick();

            var battle = _game.BattleService.GetRunningBattle(party.Components.Get<BattleGroupComponent>().BattleID);
            _game.BattleService.BattleTasks[battle.ID].Tick();

            Assert.AreEqual(dungeonTile, party.GetTile());
            Assert.AreEqual(1, _player.PlayerData.BattleHeaders.Count);
        }


        [Test]
        public void TestDungeonComponentsRegisterBuilding()
        {
            var playerCastleTile = _player.Buildings.First().GetTile();
            var dungeonTile = playerCastleTile.GetNeighbor(Direction.EAST);

            _dungeon.Logic.Map.SetPosition(dungeonTile);

            var tileBuilding = dungeonTile.Components.Get<TileHabitantsComponent>().Building;
            Assert.That(tileBuilding.EntityId == _dungeon.EntityId);
            Assert.That(tileBuilding.Components.Get<BattleGroupComponent>().Units.Valids > 0);
        }

        [Test]
        public void TestDungeonSpawnsWithMaxHPUnits()
        {
            var team = _dungeon.Components.Get<BattleGroupComponent>().Units;

            var unit = team[0];

            Assert.That(unit.HP == unit.MaxHP && unit.HP > 0);
        }

        [Test]
        public void TestDungeonInitialization()
        {
            Assert.That(_dungeon.GetTile() != null);
            Assert.That(_dungeon.Get<BattleGroupComponent>().Units.Valids > 0);
        }

        [Test]
        public void TestDungeonRemovedWhenComplete()
        {
            var playerCastleTile = _player.Buildings.First().GetTile();
            var dungeonTile = playerCastleTile.GetNeighbor(Direction.EAST);
            var party = _player.GetParty(0);

            var component = party.Components.Get<BattleGroupComponent>();
            var unit = component.Units[0];
            unit.Atk = 255;
            component.Units[0] = unit;
            party.Save(component);

            _dungeon.Logic.Map.SetPosition(dungeonTile);

            _player.SendMoveRequest(party, dungeonTile, CourseIntent.OffensiveTarget);
            _player.GetParty(0).Course().Tick();

            var battle = _game.BattleService.GetRunningBattle(party.Components.Get<BattleGroupComponent>().BattleID);
            _game.BattleService.BattleTasks[battle.ID].Tick();

            Assert.IsTrue(_dungeon.Logic.BattleGroup.IsDestroyed);
            Assert.AreEqual(_dungeon.GetTile(), null);
            Assert.AreEqual(dungeonTile.Components.Get<TileHabitantsComponent>().Building, null);
            Assert.AreEqual(_player.ReceivedPacketsOfType<EntityDestroyPacket>().Count, 1);
        }

        [Test]
        public void TestPartyTeleportsBackAfterLoosingToDungeon()
        {
            var playerCastleTile = _player.Buildings.First().GetTile();
            var dungeonTile = playerCastleTile.GetNeighbor(Direction.EAST);
            var party = _player.GetParty(0);

            var component = _dungeon.Components.Get<BattleGroupComponent>();
            var unit = component.Units[0];
            unit.Atk = 255;
            unit.Speed = 255;
            component.Units[0] = unit;
            _dungeon.Save(component);

            _dungeon.Logic.Map.SetPosition(dungeonTile);

            _player.SendMoveRequest(party, dungeonTile, CourseIntent.OffensiveTarget);
            _player.GetParty(0).Course().Tick();

            var battle = _game.BattleService.GetRunningBattle(party.Components.Get<BattleGroupComponent>().BattleID);
            _game.BattleService.BattleTasks[battle.ID].Tick();

            Assert.AreEqual(party.GetTile(), playerCastleTile);
            Assert.IsTrue(!party.Logic.BattleGroup.IsDestroyed);
            Assert.AreEqual(_dungeon.GetTile(), dungeonTile);
            Assert.AreEqual(dungeonTile.Components.Get<TileHabitantsComponent>().Building, _dungeon);
            Assert.AreEqual(_player.ReceivedPacketsOfType<EntityDestroyPacket>().Count, 0);
        }

        [Test]
        public void TestRevealingDungeonReceivesPacket()
        {
            _dungeon = _game.Entities.CreateEntity(EntityType.Dungeon); // 15 10 / 12 10
            _dungeon.Get<BattleGroupComponent>().Units.Add(new Unit(_game.Specs.Units[1]));
            _dungeon.Logic.Map.SetPosition(_game.World.GetTile(_party.GetTile().X + _party.GetLineOfSight() + 1, _party.GetTile().Y));

            _game.Network.DeltaCompression.ClearDeltas();

            var seenClose = _dungeon.GetTile().GetNeighbor(Direction.WEST);

            Assert.That(seenClose.Logic.Vision.GetEntitiesViewing().Contains(_party.EntityId));
            Assert.That(!_dungeon.GetTile().Logic.Vision.GetEntitiesViewing().Contains(_party.EntityId));

            _player.ReceivedPackets.Clear();
            _party.Logic.Map.SetPosition(_party.GetTile().GetNeighbor(Direction.EAST));
            _game.Network.DeltaCompression.SendAllModifiedEntities(_player.EntityId);

            var dungeonTile = _dungeon.GetTile();
            Assert.That(_dungeon.GetTile().Logic.Vision.GetEntitiesViewing().Contains(_party.EntityId));
            Assert.That(_player.ReceivedPacketsOfType<EntityUpdatePacket>().Any(p => p.Type == EntityType.Dungeon));
            Assert.That(_dungeon.GetTile().Logic.Vision.GetEntitiesViewing().Contains(_party.EntityId));
        }

        [Test]
        public void TestDungeonBattleTeamHasUnits()
        {
            _dungeon = _game.Entities.CreateEntity(EntityType.Dungeon);
            _dungeon.Get<BattleGroupComponent>().Units.Add(new Unit(_game.Specs.Units[1]));

            var battleTeam = new BattleTeam(new BattleTeamData(_dungeon));
            Assert.AreNotEqual(battleTeam.Units.Length, 0);
        }

        [Test]
        public void TestReceivingDungeonWhenLogin()
        {
            var joinEvent = new JoinWorldMapCommand();
            var clientPlayer = new TestServerPlayer(_game);
            _player.ReceivedPackets.Clear();

            var dg = _game.Entities.CreateEntity(EntityType.Dungeon);
            dg.Logic.Map.SetPosition(_game.World.GetTile(4, 2));

            _game.HandleClientEvent(clientPlayer, joinEvent);

            var entityVisibleEvents = clientPlayer.ReceivedPacketsOfType<EntityUpdatePacket>().Where(e => e.Type != EntityType.Tile).ToList();

            Assert.That(entityVisibleEvents.Any(p => p.Type == EntityType.Dungeon));
            Assert.AreEqual(3, entityVisibleEvents.Count, "Initial Party & Building should be visible");
        }

        [Test]
        public void TestPartyDefeatHealsDungeon()
        {
            var playerCastleTile = _player.Buildings.First().GetTile();
            var dungeonTile = playerCastleTile.GetNeighbor(Direction.EAST);
            var party = _player.GetParty(0);

            var component = party.Components.Get<BattleGroupComponent>();
            var unit = component.Units[0];
            unit.Atk = 1;
            unit.HP = 1;
            component.Units[0] = unit;
            party.Save(component);

            dungeonTile.Components.Get<TileHabitantsComponent>().Building = _dungeon;

            _player.SendMoveRequest(_player.GetParty(0), dungeonTile, CourseIntent.OffensiveTarget);
            _player.GetParty(0).Course().Tick();

            var b = _game.BattleService.GetRunningBattle(party.Get<BattleGroupComponent>().BattleID);
            _game.BattleService.BattleTasks[b.ID].Tick();
        }
    }
}