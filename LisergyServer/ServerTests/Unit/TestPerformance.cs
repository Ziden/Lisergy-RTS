using BattleServer;
using Game;
using Game.Entity;
using Game.Events.ServerEvents;
using Game.Movement;
using Game.Scheduler;
using Game.World;
using GameDataTest;
using NUnit.Framework;
using ServerTests;
using System.Collections.Generic;
using System.Linq;

namespace Tests
{
    public class TestPerformance
    {


        [Test]
        public void Test0Initial()
        {
            var game = new TestGame();
        }

        [Test]
        public void TestCreateGame()
        {
            var game = new TestGame();
        }


        [Test]
        public void TestCreateGame2()
        {
            var game = new TestGame();
        }

        [Test]
        public void TestNoCreateGame()
        {
            var game = new TestGame(createPlayer: false);
        }

        [Test]
        public void TestNoCreateGame2()
        {
            var game = new TestGame(createPlayer: false);
        }


    }
}