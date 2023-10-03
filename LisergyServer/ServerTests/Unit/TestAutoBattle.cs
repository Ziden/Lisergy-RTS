using Game;
using Game.Battle;
using Game.Battle.BattleActions;
using Game.Battle.BattleEvents;
using Game.DataTypes;
using Game.ECS;
using Game.Network;
using Game.Systems.Battler;
using Game.Systems.Party;
using Game.Systems.Player;
using NUnit.Framework;
using ServerTests;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Tests
{
    public class TestAutoBattle
    {
        private TestGame Game;

        [SetUp]
        public void Setup()
        {
            Game = new TestGame();
        }


        [Test]
        public void TestBattleComponentLogicSync()
        {
            var clientEntity = new PartyEntity(null);

            var clientComponent = clientEntity.Components.Add<BattleGroupComponent>();
            var serverComponent = new BattleGroupComponent();
            serverComponent.BattleID = GameId.Generate();

            ComponentSynchronizer.SyncComponents(clientEntity, new List<IComponent>() { serverComponent });

            Assert.AreEqual(clientComponent.BattleID, serverComponent.BattleID);
        }

        [Test]
        public void TestLogicTriggeringEvents()
        {
            var enemyTeam = new BattleTeam(new Unit(0).SetBaseStats(), new Unit(0).SetBaseStats());
            var myTeam = new BattleTeam(new Unit(2).SetBaseStats(), new Unit(0).SetBaseStats());
            var battle = new TurnBattle(Guid.NewGuid(), myTeam, enemyTeam);
            var autoRun = new AutoRun(battle);
            var result = autoRun.RunAllRounds();

            Assert.IsTrue(result.Turns.Count > 10);
            Assert.IsTrue(result.Winner != null);
        }

        [Test]
        public void TestDeadDontAct()
        {
            var weak = new Unit(0).SetBaseStats();
            var enemyTeam = new BattleTeam(weak, new Unit(0).SetBaseStats());

            var op = TestBattle.MakeOverpower(new Unit(0).SetBaseStats());
            var myTeam = new BattleTeam(op, new Unit(0).SetBaseStats());

            var battle = new TurnBattle(Guid.NewGuid(), myTeam, enemyTeam);
            var autoRun = new AutoRun(battle);

            List<BattleEvent> events = null;
            UnitDeadEvent deathEvent = null;
            var turn = 0;

            while(deathEvent == null && turn < 10)
            {
                var result = autoRun.RunOneTurn();
                deathEvent = result.FirstOrDefault(e => e is UnitDeadEvent) as UnitDeadEvent;
                turn++;
            }

            while(!battle.IsOver)
            {
                var result = autoRun.RunOneTurn();
                Assert.False(result.Where(e => e is AttackAction).Cast<AttackAction>().Any(a => a.UnitID == deathEvent.UnitId), "Dead units dont attack");
            }

            Assert.NotNull(deathEvent, "Death event not fired");
        }
    }
}
