using Game;
using Game.Events.ServerEvents;
using NUnit.Framework;
using ServerTests;
using System;
using System.Linq;
using Game.Systems.Player;
using Game.Systems.Resources;

namespace UnitTests
{
    public class TestResources
    {
        private TestGame _game;
        private PlayerEntity _player;

        [SetUp]
        public void Setup()
        {
            _game = new TestGame();
            _player = _game.GetTestPlayer();
        }
        
        [Test]
        public void TestSaveComponent()
        {
            var c = new CargoComponent();
            _player.Parties.First().Save(c);
        }
    }
}