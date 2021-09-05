using Game;
using Game.Events.ClientEvents;
using Game.Events.ServerEvents;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tests
{
    public class TestDungeon
    {
        private TestGame Game;

        [SetUp]
        public void Setup()
        {
            Game = new TestGame();
        }

        [TearDown]
        public void TearDown()
        {

        }

        [Test]
        public void TestCreateParty()
        {
            var player = Game.GetTestPlayer();
            var unit = new Unit()
            {
                Name = "TestUnit"
            };

        }
    }
}
