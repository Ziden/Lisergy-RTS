using Game.Engine.DataTypes;
using Game.Systems.Battle;
using Game.Systems.Battle.BattleActions;
using Game.Systems.Battle.BattleEvents;
using Game.Systems.Battle.Data;
using Game.Systems.Battler;
using NUnit.Framework;
using ServerTests;
using System.Collections.Generic;
using System.Linq;

namespace GameUnitTests
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
        public void TestLogicTriggeringEvents()
        {
            var enemyTeam = new BattleGroupData(new Unit(Game.Specs.Units[0]), new Unit(Game.Specs.Units[0]));
            var myTeam = new BattleGroupData(new Unit(Game.Specs.Units[2]), new Unit(Game.Specs.Units[0]));
            var battle = new TurnBattle(GameId.Generate(), myTeam, enemyTeam);
            var autoRun = new AutoRun(battle);
            var result = autoRun.RunAllRounds();

            Assert.IsTrue(result.Turns.Count > 3);
            Assert.IsTrue(result.Winner != null);
        }

        [Test]
        public unsafe void TestUnitDie()
        {
            var weak = new Unit(Game.Specs.Units[0]);
            var enemyTeam = new BattleGroupData(weak, new Unit(Game.Specs.Units[0]));

            var unit = new Unit(Game.Specs.Units[0]);
            var op = TestBattle.MakeOverpower(ref unit);
            var myTeam = new BattleGroupData(op, new Unit(Game.Specs.Units[0]));

            var battle = new TurnBattle(GameId.Generate(), myTeam, enemyTeam);
            var autoRun = new AutoRun(battle);

            UnitDeadEvent deathEvent = null;
            var turn = 0;

            while (deathEvent == null && turn < 10)
            {
                var result = autoRun.RunOneTurn();
                deathEvent = result.FirstOrDefault(e => e is UnitDeadEvent) as UnitDeadEvent;
                turn++;
            }

            var deadUnit = battle.FindUnit(deathEvent.UnitId);

            Assert.NotNull(deathEvent, "Death event not fired");
            Assert.IsTrue(deadUnit.Stats.HP == 0, "Unit is dead");
        }

        [Test]
        public void TestDeadDontAct()
        {
            var weak = new Unit(Game.Specs.Units[0]);
            var enemyTeam = new BattleGroupData(weak, new Unit(Game.Specs.Units[0]));

            var unit = new Unit(Game.Specs.Units[0]);
            var op = TestBattle.MakeOverpower(ref unit);
            var myTeam = new BattleGroupData(op, new Unit(Game.Specs.Units[0]));

            var battle = new TurnBattle(GameId.Generate(), myTeam, enemyTeam);
            var autoRun = new AutoRun(battle);

            UnitDeadEvent deathEvent = null;
            var turn = 0;

            while (deathEvent == null && turn < 10)
            {
                var result = autoRun.RunOneTurn();
                deathEvent = result.FirstOrDefault(e => e is UnitDeadEvent) as UnitDeadEvent;
                turn++;
            }

            while (!battle.IsOver)
            {
                var result = autoRun.RunOneTurn();
                Assert.False(result.Where(e => e is AttackAction).Cast<AttackAction>().Any(a => a.UnitID == deathEvent.UnitId), "Dead units dont attack");
            }

            Assert.NotNull(deathEvent, "Death event not fired");
        }
    }
}
