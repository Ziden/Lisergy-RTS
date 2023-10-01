using Game.Battle;
using Game.Battle.BattleActions;
using Game.Systems.Battler;
using NUnit.Framework;
using System;
using System.Linq;

namespace Tests
{
    public class TestBattleActions
    {
        private Unit FastUnit;
        private Unit SlowUnit;
        private TurnBattle Battle;

        [SetUp]
        public void Setup()
        {
            FastUnit = new Unit(1);
            FastUnit.Name = "Fast Unit";
            FastUnit.Speed *= 2;

            SlowUnit = new Unit(1);
            SlowUnit.Name = "Slow Unit";
            SlowUnit.Speed /= 2;

            Battle = new TurnBattle(Guid.NewGuid(), new BattleTeam(FastUnit), new BattleTeam(SlowUnit));
        }

        [Test]
        public void TestAttackAction()
        {
            var attacker = Battle.CurrentActingUnit;
            var defender = Battle.GetOpposingTeam(attacker).Units.First();

            var action = new AttackAction(Battle, attacker, defender);
            Battle.ReceiveAction(action);

            var result = action.Result as AttackActionResult;

            Assert.NotNull(result);
            Assert.That(defender.UnitReference.HP == defender.UnitReference.MaxHP - result.Damage);
        }
    }
}