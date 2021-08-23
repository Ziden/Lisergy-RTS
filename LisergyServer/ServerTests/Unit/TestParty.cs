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

            var enemy = new Dungeon();
            enemy.AddBattle(new Unit(0));

            var battleID = Guid.NewGuid().ToString();
            _game.NetworkEvents.Call(new BattleStartPacket()
            {
                Attacker = party.GetBattleTeam(),
                Defender = enemy.GetBattleTeam(),
                BattleID = battleID
            });

            party.Battle.Task.Execute();

            var statusUpdates = _player.ReceivedEventsOfType<PartyStatusUpdatePacket>();
            Assert.AreEqual(1, statusUpdates.Count);
        }
    }
}