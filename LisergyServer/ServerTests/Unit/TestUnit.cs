using Game.Events.ServerEvents;
using Game.Specs;
using Game.Systems.Battler;
using NUnit.Framework;
using ServerTests;
using System.Linq;
using Tests.Unit.Stubs;

namespace GameUnitTests
{
    public class TestUnits
    {
        private TestGame Game;

        [SetUp]
        public void SetupTests()
        {
            Game = new TestGame();
        }

        [Test]
        public void TestInitialUnit()
        {
            var player = Game.GetTestPlayer();
            var party = player.Parties.First();

            Assert.AreEqual(0, player.PlayerData.StoredUnits.Count);
            Assert.AreEqual(Game.Specs.InitialUnitSpecId, party.Get<BattleGroupComponent>().Units.Leader.SpecId);
            Assert.That(party.Logic.Map.GetTile().Logic.Vision.GetEntitiesViewing().Contains(party.EntityId));
        }

        [Test]
        public void TestUnitStatsEquality()
        {
            var s1 = UnitStats.DEFAULT;
            var s2 = UnitStats.DEFAULT;

            s1.HP = 123;
            s2.HP = 123;

            Assert.AreEqual(s1, s2);
        }

        [Test]
        public void TestUnitStatsNonEquality()
        {
            var s1 = UnitStats.DEFAULT;
            var s2 = UnitStats.DEFAULT;

            s1.HP = 12;
            s2.HP = 123;

            Assert.AreNotEqual(s1, s2);
        }

        [Test]
        public void TestUnitTileReference()
        {
            var player = Game.GetTestPlayer();

            var party = player.Parties.First();

            var tile = Game.World.GetTile(party.GetTile().X, party.GetTile().Y);

            Assert.AreEqual(tile, party.GetTile());
        }

        [Test]
        public void TestUnitStatSetter()
        {
            var unit = new Unit(Game.Specs.Units[0]);

            unit.MaxHP = 50;
            unit.Speed = 55;

            Assert.AreEqual(50, unit.MaxHP);
            Assert.AreEqual(55, unit.Speed);
        }

        [Test]
        public void TestUnitVisibleEvent()
        {
            var player = Game.GetTestPlayer();
            var building = player.Buildings.First();
            var party = player.Parties.First();

            var tile = party.GetTile();

            var visibleEvent = Game.SentServerPackets.Where(e => e is EntityUpdatePacket && ((EntityUpdatePacket)e).EntityId == party.EntityId).FirstOrDefault() as EntityUpdatePacket;

            Assert.That(visibleEvent != null);
            Assert.AreEqual(party.EntityId, visibleEvent.EntityId);
        }
    }
}