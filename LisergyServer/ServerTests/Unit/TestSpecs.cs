using BattleServer;
using Game;
using Game.Entity;
using Game.Events;
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
    public class TestSpecSerialization
    {
      
        [SetUp]
        public void Setup()
        {
            
        }

        [TearDown]
        public void Tear()
        {
          
        }

        [Test]
        public void TestBasicSerialization()
        {
            var game = new TestGame();
            //var serialized = Serialization.FromEventRaw(new GameSpecPacket(game));
            //var deserialized = (GameSpecPacket)Serialization.ToEventRaw(serialized);

            var asd = 123;
        }
    }
}