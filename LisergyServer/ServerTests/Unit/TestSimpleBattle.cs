using Game.Engine;
using Game.Engine.DataTypes;
using Game.Engine.ECLS;
using Game.Engine.Network;
using Game.Network.ServerPackets;
using Game.Systems.Battle;
using Game.Systems.Battle.BattleActions;
using Game.Systems.Battle.Data;
using Game.Systems.Battler;
using GameDataTest;
using NUnit.Framework;
using ServerTests;
using System;
using System.Linq;
using System.Text.Json;

namespace GameUnitTests
{
    public unsafe class TestAutobattles
    {
        private Unit StrongUnit;
        private Unit WeakUnit;
        private Unit FastUnit;
        private Unit SlowUnit;

        [SetUp]
        public void Setup()
        {
            var specs = TestSpecs.Generate();
            StrongUnit = new Unit(specs.Units[1]);
            StrongUnit.Stats.Atk *= 4;

            WeakUnit = new Unit(specs.Units[1]);

            FastUnit = new Unit(specs.Units[1]);
            FastUnit.Stats.Speed *= 10;

            SlowUnit = new Unit(specs.Units[1]);
            SlowUnit.Stats.Speed /= 2;
        }

        [Test]
        public void TestUnitsOrderingSameSpeed()
        {
            var battle = new TestBattle(new BattleGroupData(StrongUnit), new BattleGroupData(WeakUnit));
            var first = battle.NextUnitToAct;

            Assert.AreEqual(first.RT, first.MaxRT);

            battle.AutoRun.RunOneTurn();

            var second = battle.NextUnitToAct;

            Assert.AreNotEqual(first, second);
            Assert.AreEqual(first.MaxRT * 2, first.RT);
            Assert.AreEqual(second.MaxRT, second.RT);
        }

        [Test]
        public void TestFasterActFirst()
        {
            var battle = new TestBattle(new BattleGroupData(WeakUnit), new BattleGroupData(FastUnit));

            Assert.AreEqual(battle.NextUnitToAct.UnitID, FastUnit.Id);

            var lastAction = battle.AutoRun.RunOneTurn().Last() as BattleAction;

            Assert.AreEqual(lastAction.Unit.UnitID, FastUnit.Id);
            Assert.AreEqual(lastAction.Unit.RT, lastAction.Unit.MaxRT * 2);
        }

        [Test]
        public void TestUnitDelay()
        {
            var battle = new TestBattle(new BattleGroupData(FastUnit), new BattleGroupData(WeakUnit));
            var result = battle.AutoRun.RunAllRounds();

            var fastAttacks = result.Turns.Where(r => r.Events.Any(a => a is BattleAction && ((BattleAction)a).Unit.UnitID == FastUnit.Id)).ToList();
            var weakAttacks = result.Turns.Where(r => r.Events.Any(a => a is BattleAction && ((BattleAction)a).Unit.UnitID == WeakUnit.Id)).ToList();

            Assert.That(fastAttacks.Count() > weakAttacks.Count());
        }

        [Test]
        public void TestDelayProportion()
        {
            FastUnit.Stats.HP = 60;

            SlowUnit.Stats.HP = 60;

            var group1 = new BattleGroupData(FastUnit);
            var group2 = new BattleGroupData(SlowUnit);
            var battle = new TestBattle(group1, group2);
            var result = battle.AutoRun.RunAllRounds();

            var fastAttacks = result.Turns.Where(r => r.Events.Any(a => a is BattleAction && ((BattleAction)a).Unit.UnitID == FastUnit.Id)).ToList();
            var slowAttacks = result.Turns.Where(r => r.Events.Any(a => a is BattleAction && ((BattleAction)a).Unit.UnitID == SlowUnit.Id)).ToList();

            Assert.That(fastAttacks.Count() > slowAttacks.Count());
        }

        [Test]
        public void TestWinner()
        {
            var battle = new TurnBattle(GameId.Generate(), new BattleGroupData(StrongUnit), new BattleGroupData(WeakUnit));
            var result = battle.AutoRun.RunAllRounds();

            Assert.AreEqual(result.Winner, result.Attacker);
        }


        [Test]
        public void TestUnitsBeingUpdated()
        {
            var initialHP = StrongUnit.Stats.HP;
            var battle = new TurnBattle(GameId.Generate(), new BattleGroupData(StrongUnit), new BattleGroupData(WeakUnit));
            var result = battle.AutoRun.RunAllRounds();

            var finalHP = result.Attacker.Units[0].UnitData.Stats.HP;
            Assert.AreNotEqual(initialHP, finalHP);
        }

        [Test]
        public void TestSerializeGameId()
        {
            Serialization.LoadSerializers();

            var id = GameId.Generate();

            var bytes = Serialization.FromAnyType(id);

            var idJson = JsonSerializer.Serialize(id);

            var back = Serialization.ToAnyType<GameId>(bytes);
            Assert.AreEqual(id, back);
        }

        [Test]
        public void TestSerialization()
        {
            Serialization.LoadSerializers();
            var battle = new TurnBattle(GameId.Generate(), new BattleGroupData(StrongUnit), new BattleGroupData(WeakUnit));
            var result = battle.AutoRun.RunAllRounds();

            var ev = new BattleResultPacket(battle.ID, result);

            var bytes = Serialization.FromAnyType(ev);
            ev = Serialization.ToAnyType<BattleResultPacket>(bytes);

            Assert.AreEqual(ev.Header.Attacker.Units[0].Id, result.Attacker.Units.First().UnitID);
        }
    }
}