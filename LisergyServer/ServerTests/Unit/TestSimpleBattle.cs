using Game;
using Game.Battle;
using Game.Battle.BattleActions;
using Game.Events;
using Game.Network.ServerPackets;
using Game.Systems.Battler;
using NUnit.Framework;
using ServerTests;
using System;
using System.Linq;

namespace Tests
{
    public class TestAutobattles
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
            StrongUnit.Atk *= 4;

            WeakUnit = new Unit(1);
            WeakUnit.Name = "Weak Unit";

            FastUnit = new Unit(1);
            FastUnit.Name = "Fast Unit";
            FastUnit.Speed *= 2;

            SlowUnit = new Unit(1);
            SlowUnit.Name = "Slow Unit";
            SlowUnit.Speed /= 2;
        }

        [Test]
        public void TestUnitsOrderingSameSpeed()
        {
            var battle = new TestBattle(new BattleTeam(StrongUnit), new BattleTeam(WeakUnit));
            var first = battle.NextUnitToAct;

            Assert.AreEqual(first.RT, first.GetMaxRT());

            battle.AutoRun.RunOneTurn();

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

            var lastAction = battle.AutoRun.RunOneTurn().Last() as BattleAction;

            Assert.AreEqual(lastAction.Unit.UnitID, FastUnit.Id);
            Assert.AreEqual(lastAction.Unit.RT, lastAction.Unit.GetMaxRT() * 2);
        }

        [Test]
        public void TestUnitDelay()
        {
            var battle = new TestBattle(new BattleTeam(FastUnit), new BattleTeam(WeakUnit));
            var result = battle.AutoRun.RunAllRounds();

            var fastAttacks = result.Turns.Where(r => r.Events.Any(a => a is BattleAction && ((BattleAction)a).Unit.UnitID == FastUnit.Id)).ToList();
            var weakAttacks = result.Turns.Where(r => r.Events.Any(a => a is BattleAction && ((BattleAction)a).Unit.UnitID == WeakUnit.Id)).ToList();

            Assert.That(fastAttacks.Count() > weakAttacks.Count());
        }

        [Test]
        public void TestDelayProportion()
        {
            FastUnit.Speed = 10;
            FastUnit.HP = 50;

            SlowUnit.Speed = 5;
            SlowUnit.HP = 50;

            var battle = new TestBattle(new BattleTeam(FastUnit), new BattleTeam(SlowUnit));
            var result = battle.AutoRun.RunAllRounds();

            var fastAttacks = result.Turns.Where(r => r.Events.Any(a => a is BattleAction && ((BattleAction)a).Unit.UnitID == FastUnit.Id)).ToList();
            var slowAttacks = result.Turns.Where(r => r.Events.Any(a => a is BattleAction && ((BattleAction)a).Unit.UnitID == SlowUnit.Id)).ToList();

            Assert.AreEqual(50, fastAttacks.Count());
            Assert.That(slowAttacks.Count() < 49);
        }

        [Test]
        public void TestWinner()
        {
            var battle = new TurnBattle(Guid.NewGuid(), new BattleTeam(StrongUnit), new BattleTeam(WeakUnit));
            var result = battle.AutoRun.RunAllRounds();

            Assert.AreEqual(result.Winner, result.Attacker);
        }


        [Test]
        public void TestUnitsBeingUpdated()
        {
            var initialHP = StrongUnit.HP;
            var battle = new TurnBattle(Guid.NewGuid(), new BattleTeam(StrongUnit), new BattleTeam(WeakUnit));
            var result = battle.AutoRun.RunAllRounds();

            var finalHP = StrongUnit.HP;
            Assert.AreNotEqual(initialHP, finalHP);
        }

        [Test]
        public void TestSerialization()
        {
            Serialization.LoadSerializers();
            var battle = new TurnBattle(Guid.NewGuid(), new BattleTeam(StrongUnit), new BattleTeam(WeakUnit));
            var result = battle.AutoRun.RunAllRounds();

            var ev = new BattleResultPacket(battle.ID, result);

            var bytes = Serialization.FromEvent(ev);
            ev = Serialization.ToEvent<BattleResultPacket>(bytes);

            Assert.AreEqual(ev.FinalStateHeader.Attacker.Units.First().UnitID, result.Attacker.Units.First().UnitID);
        }
    }
}