using Game;
using Game.Battle;
using Game.Events.ServerEvents;
using Game.Network.ClientPackets;
using Game.Network.ServerPackets;
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

namespace UnitTests
{
    public class TestMap
    {
        private TestGame _game;
        private TestServerPlayer _player;
        private PartyEntity _party;

        [SetUp]
        public void Setup()
        {
            _game = new TestGame();
            _player = _game.GetTestPlayer();
            _party = _player.GetParty(0);
        }

        [Test]
        public void TestMapLogicSetPosition()
        {
            var playerCastleTile = _player.Buildings.First().Tile;
            var dungeonTile = playerCastleTile.GetNeighbor(Direction.SOUTH);
            var party = _player.GetParty(0);

            //Assert.That(party.Tile == _game)
            party.EntityLogic.Map.SetPosition(dungeonTile);
        }
    }
}