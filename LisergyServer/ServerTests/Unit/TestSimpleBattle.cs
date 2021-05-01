using Game;
using Game.Battles;
using Game.BattleTactics;
using Game.Events;
using NUnit.Framework;
using ServerTests;
using System.Linq;

namespace Tests
{
    public class TestBattles
    {
        private Unit StrongUnit;
        private Unit WeakUnit;
        private Unit FastUnit;
        private Unit SlowUnit;

        [SetUp]
        public void Setup()
        {
            StrongUnit = new Unit(1);
            StrongUnit.Name = "Strong Unit";
            StrongUnit.Stats.Atk *= 4;

            WeakUnit = new Unit(1);
            WeakUnit.Name = "Weak Unit";

            FastUnit = new Unit(1);
            FastUnit.Name = "Fast Unit";
            FastUnit.Stats.Speed *= 2;

            SlowUnit = new Unit(1);
            SlowUnit.Name = "Slow Unit";
            SlowUnit.Stats.Speed /= 2;
        }

        [Test]
        public void TestDamage()
        {
            var t1 = new BattleTeam(StrongUnit);
            var t2 = new BattleTeam(WeakUnit);

            var strong = t1.Units.First();
            var weaks = t2.Units.First();

            var currentHP = weaks.Stats.HP;
            var damage = strong.Stats.Atk - (weaks.Stats.Def / 2);

            var action = strong.Attack(weaks);

            Assert.AreEqual(weaks.Stats.HP, currentHP - damage);
            Assert.AreEqual(((AttackActionResult)action.Result).Damage, damage);
        }

        [Test]
        public void TestUnitsOrderingSameSpeed()
        {
            var battle = new TestBattle(new BattleTeam(StrongUnit), new BattleTeam(WeakUnit));
            var first = battle.NextUnitToAct;

            Assert.AreEqual(first.RT, first.GetMaxRT());

            battle.RunSingleTurn();

            var second = battle.NextUnitToAct;

            Assert.AreNotEqual(first, second);
            Assert.AreEqual(first.GetMaxRT() * 2, first.RT);
            Assert.AreEqual(second.GetMaxRT(), second.RT);
        }

        [Test]
        public void TestFasterActFirst()
        {
            var battle = new TestBattle(new BattleTeam(WeakUnit), new BattleTeam(FastUnit));

            Assert.AreEqual(battle.NextUnitToAct.UnitID, FastUnit.Id);

            var lastAction = battle.RunSingleTurn();
            Assert.AreEqual(lastAction.Unit.UnitID, FastUnit.Id);
            Assert.AreEqual(lastAction.Unit.RT, lastAction.Unit.GetMaxRT() * 2);
        }

        [Test]
        public void TestUnitDelay()
        {
            var battle = new TestBattle(new BattleTeam(FastUnit), new BattleTeam(WeakUnit));
            var result = battle.Run();

            var fastAttacks = result.Turns.Where(r => r.Actions.Any(a => a.Unit.UnitID == FastUnit.Id)).ToList();
            var weakAttacks = result.Turns.Where(r => r.Actions.Any(a => a.Unit.UnitID == WeakUnit.Id)).ToList();

            Assert.That(fastAttacks.Count() > weakAttacks.Count());
        }

        [Test]
        public void TestDelayProportion()
        {
            FastUnit.Stats.Speed = 10;
            FastUnit.Stats.HP = 50;

            SlowUnit.Stats.Speed = 5;
            SlowUnit.Stats.HP = 50;

            var battle = new TestBattle(new BattleTeam(FastUnit), new BattleTeam(SlowUnit));
            var result = battle.Run();

            var fastAttacks = result.Turns.Where(r => r.Actions.Any(a => a.Unit.UnitID == FastUnit.Id)).ToList();
            var slowAttacks = result.Turns.Where(r => r.Actions.Any(a => a.Unit.UnitID == SlowUnit.Id)).ToList();

            Assert.AreEqual(50, fastAttacks.Count());
            Assert.That(slowAttacks.Count() < 49);
        }

        [Test]
        public void TestWinner()
        {
            var battle = new TurnBattle(new BattleTeam(StrongUnit), new BattleTeam(WeakUnit));
            var result = battle.Run();

            Assert.AreEqual(result.Winner, result.Attacker);
        }

        [Test]
        public void TestSerialization()
        {
            Serialization.LoadSerializers();
            var battle = new TurnBattle(new BattleTeam(StrongUnit), new BattleTeam(WeakUnit));
            var result = battle.Run();

            var ev = new BattleResultCompleteEvent(result);

            var bytes = Serialization.FromEvent(ev);
            ev = Serialization.ToEvent<BattleResultCompleteEvent>(bytes);

            Assert.AreEqual(ev.Attacker.Units.First().UnitID, result.Attacker.Units.First().UnitID);
        }
    }
}