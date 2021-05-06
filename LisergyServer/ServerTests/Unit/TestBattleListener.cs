using BattleServer;
using Game;
using Game.Battles;
using Game.Battles.Actions;
using Game.Events;
using Game.Events.ClientEvents;
using NUnit.Framework;
using ServerTests;
using System;

namespace Tests
{
    public class TestBattleListener
    {
        private Unit FastUnit;
        private Unit SlowUnit;
        private BattleListener Listener;

        [SetUp]
        public void Setup()
        {
            FastUnit = new Unit(1);
            FastUnit.Name = "Fast Unit";
            FastUnit.Stats.Speed *= 2;

            SlowUnit = new Unit(1);
            SlowUnit.Name = "Slow Unit";
            SlowUnit.Stats.Speed /= 2;

            var game = new TestGame();
            Listener = game.GetListener<BattleListener>();
        }

        [Test]
        public void TestCreatingBattle()
        {
            Listener.OnBattleStart(new BattleStartEvent()
            {
                Attacker = new BattleTeam(FastUnit),
                Defender = new BattleTeam(SlowUnit),
                BattleID = Guid.NewGuid().ToString()
            });

            Assert.AreEqual(Listener.BattleCount(), 1);

            Listener.OnBattleStart(new BattleStartEvent()
            {
                Attacker = new BattleTeam(FastUnit),
                Defender = new BattleTeam(SlowUnit),
                BattleID = Guid.NewGuid().ToString()
            });

            Assert.AreEqual(Listener.BattleCount(), 2);
        }

        [Test]
        public void TestActing()
        {
            var id = Guid.NewGuid().ToString();
            Listener.OnBattleStart(new BattleStartEvent()
            {
                Attacker = new BattleTeam(FastUnit),
                Defender = new BattleTeam(SlowUnit),
                BattleID = id
            });

            var battle = Listener.GetBattle(id);
            var actingUnit = battle.CurrentActingUnit;
            var atk = new AttackAction(actingUnit, battle.GetOpposingTeam(actingUnit).RandomUnit());
            Listener.OnBattleAction(new BattleActionEvent(id, atk));

            Assert.That(battle._log.Turns.Count == 1);

            atk = new AttackAction(actingUnit, battle.GetOpposingTeam(actingUnit).RandomUnit());
            Listener.OnBattleAction(new BattleActionEvent(id, atk));

            Assert.That(battle._log.Turns.Count == 2);
        }

        [Test]
        public void TestBattleFinishing()
        {
            var id = Guid.NewGuid().ToString();
            Listener.OnBattleStart(new BattleStartEvent()
            {
                Attacker = new BattleTeam(FastUnit),
                Defender = new BattleTeam(SlowUnit),
                BattleID = id
            });

            var battle = Listener.GetBattle(id);
            var actingUnit = battle.CurrentActingUnit;
            var atk = new AttackAction(actingUnit, battle.GetOpposingTeam(actingUnit).RandomUnit());
            Listener.OnBattleResult(new BattleResultEvent(battle._log));

            Assert.That(battle._log.Turns.Count == 1);

            atk = new AttackAction(actingUnit, battle.GetOpposingTeam(actingUnit).RandomUnit());
            Listener.OnBattleAction(new BattleActionEvent(id, atk));

            Assert.That(battle._log.Turns.Count == 2);
        }
    }
}