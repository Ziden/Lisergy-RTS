using Game;
using Game.Entity;
using Game.Events;
using Game.Events.ServerEvents;
using Game.Scheduler;
using Game.World;
using NUnit.Framework;
using ServerTests;
using System;
using System.Linq;

namespace Tests
{
    public class TestParty
    {
        private TestGame _game;
        private TestServerPlayer _player;

        [SetUp]
        public void Setup()
        {
            _game = new TestGame();
            _player = _game.GetTestPlayer();
            GameScheduler.Clear();
        }

        [TearDown]
        public void Tear()
        {
            _game.ClearEventListeners();
        }

        [Test]
        public void TestPartyStatusUpdatedAfterBattle()
        {
            var playerCastleTile = _player.Buildings.First().Tile;
            var dungeonTile = playerCastleTile.GetNeighbor(Direction.EAST);
            var party = _player.GetParty(0);
            party.Tile = _game.World.GetTile(0, 0);
            var enemy = new Dungeon();
            enemy.Tile = _game.World.GetTile(1, 1);
            enemy.AddBattle(new Unit(0));

            var battleID = Guid.NewGuid();
            _game.NetworkEvents.Call(new BattleStartPacket(battleID, party, enemy));

            var battle = _game.BattleService.GetBattle(battleID);
            battle.Task.Execute();

            var statusUpdates = _player.ReceivedEventsOfType<PartyStatusUpdatePacket>();
            Assert.AreEqual(1, statusUpdates.Count);
        }

        [Test]
        public void TestPartyDoesNotReceiveDestroyPacket()
        {
            var playerCastleTile = _player.Buildings.First().Tile;
            var party = _player.GetParty(0);
            party.GetUnits()[0].Stats.Atk = 1;

            party.Tile = _game.World.GetTile(0, 0);

            var enemy = new Dungeon();

            enemy.Tile = _game.World.GetTile(1, 1);
            enemy.AddBattle(new Unit(0));
            enemy.Battles[0][0].Stats.Atk = 255;

            var battleID = Guid.NewGuid();
            _game.NetworkEvents.Call(new BattleStartPacket(battleID, party, enemy));

            var battle = _game.BattleService.GetBattle(battleID);
            battle.Task.Execute();

            var destroyPackets = _player.ReceivedEventsOfType<EntityDestroyPacket>();
            Assert.AreEqual(0, destroyPackets.Count);
        }
    }
}