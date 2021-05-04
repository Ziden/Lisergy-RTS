using Game;
using Game.Battles;
using Game.Battles.Actions;
using Game.BattleTactics;
using NUnit.Framework;
using System;
using System.Linq;

namespace Tests
{
    public class TestBattleActions
    {
        private Unit StrongUnit;
        private Unit WeakUnit;
        private Unit FastUnit;
        private Unit SlowUnit;
        private TurnBattle Battle;

        [SetUp]
        public void Setup()
        {
            FastUnit = new Unit(1);
            FastUnit.Name = "Fast Unit";
            FastUnit.Stats.Speed *= 2;

            SlowUnit = new Unit(1);
            SlowUnit.Name = "Slow Unit";
            SlowUnit.Stats.Speed /= 2;

            Battle = new TurnBattle(Guid.NewGuid(), new BattleTeam(FastUnit), new BattleTeam(SlowUnit));
        }

        [Test]
        public void TestAttackAction()
        {
            var attacker = Battle.CurrentActingUnit;
            var defender = Battle.GetOpposingTeam(attacker).Units.First();

            var action = new AttackAction(attacker, defender);
            Battle.ReceiveAction(action);

            var result = action.Result as AttackActionResult;

            Assert.NotNull(result);
            Assert.IsTrue(result.Succeeded);
            Assert.That(defender.Stats.HP == defender.Stats.MaxHP - result.Damage);
        }
    }
}