using Game.Engine.DataTypes;
using Game.Entities;
using Game.Systems.Battler;
using NUnit.Framework;
using ServerTests;

namespace GameUnitTests
{
    public class TestBattleGroup
    {
        private TestGame Game;

        [SetUp]
        public void Setup()
        {
            Game = new TestGame();
        }

        [Test]
        public void TestAddSingleUnit()
        {
            var e = new BaseEntity(GameId.Generate(), Game, EntityType.Party);
            e.Components.Add<BattleGroupComponent>();
            var startUnits = e.Get<BattleGroupComponent>().Units.Valids;

            e.Logic.BattleGroup.AddUnit(new Unit(Game.Specs.Units[1]));

            Assert.AreEqual(startUnits + 1, e.Get<BattleGroupComponent>().Units.Valids);
        }
    }
}
