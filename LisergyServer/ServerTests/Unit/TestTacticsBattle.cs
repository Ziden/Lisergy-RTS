using Game;
using Game.Battles;
using Game.Battles.Actions;
using Game.TacticsBattle;
using NUnit.Framework;
using ServerTests;
using System.Linq;

namespace Tests
{
    public class TestTacticsBattle
    {    /*
        private Unit StrongUnit;
        private Unit WeakUnit;
        private Unit FastUnit;
        private Unit SlowUnit;
        private PlayerEntity p1;
        private PlayerEntity p2;
        private StrategyGame Game;

    
        [SetUp]
        public void Setup()
        {
            Game = new TestGame();
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

            p1 = new TestServerPlayer();
            p2 = new TestServerPlayer();
        }

        [Test]
        public void TestMovingUnitAround()
        {
            var t1 = new BattleTeam(p1, FastUnit);
            var t2 = new BattleTeam(p2, SlowUnit);

            var map = new TacticsMap(32, 32);
            var battle = new TacticsBattle(map);

            battle.MoveUnitTo(t1.Units[0], 1, 1);
            battle.MoveUnitTo(t1.Units[0], 5, 5);

            Assert.AreEqual(0, map.GetTacticsTile(1, 1).MovingEntities.Count);
            Assert.AreEqual(t1.Units[0], map.GetTacticsTile(5, 5).MovingEntities[0]);
        }

        [Test]
        public void TestRegisteringUnitsAndTeamsOnce()
        {
            var t1 = new BattleTeam(p1, FastUnit);
            var t2 = new BattleTeam(p2, SlowUnit);

            var map = new TacticsMap(32, 32);
            var battle = new TacticsBattle(map);

            // duplicate
            battle.MoveUnitTo(t1.Units[0], 1, 1);
            battle.MoveUnitTo(t1.Units[0], 2, 2);
            // other unit
            battle.MoveUnitTo(t2.Units[0], 5, 5);

            Assert.AreEqual(2, battle.Teams.Length);
            Assert.AreEqual(2, battle.Units.Length);
            Assert.AreEqual(t1, battle.Teams[0]);
            Assert.AreEqual(t2, battle.Teams[1]);
            Assert.AreEqual(2, battle.ActionQueue.Count);
        }

        [Test]
        public void TestActionQueueOrdering()
        {
            var t1 = new BattleTeam(p1, FastUnit);
            var t2 = new BattleTeam(p2, SlowUnit);

            var map = new TacticsMap(32, 32);
            var battle = new TacticsBattle(map);

            battle.MoveUnitTo(t1.Units[0], 1, 1);
            battle.MoveUnitTo(t2.Units[0], 5, 5);

            Assert.AreEqual(2, battle.ActionQueue.Count);
            Assert.AreEqual(t1.Units.First(), battle.ActionQueue.First());
            Assert.AreEqual(t2.Units.First(), battle.ActionQueue.Last());
        }

        [Test]
        public void TestMovingUnitsInBattle()
        {
            var t1 = new BattleTeam(p1, FastUnit);
            var t2 = new BattleTeam(p2, SlowUnit);

            var map = new TacticsMap(32, 32);
            var battle = new TacticsBattle(map);

            battle.MoveUnitTo(t1.Units[0], 1, 1);
            battle.MoveUnitTo(t2.Units[0], 5, 5);

            Assert.AreEqual(battle.CurrentUnitTurn, t1.Units[0], "Fast unit should move first");

            var result = battle.ReceiveIntent(new MoveAction(t1.Units[0], 2, 2));
            Assert.AreEqual(true, result);
            Assert.AreEqual(map.GetTile(2, 2), t1.Units[0].Tile);
        }

        [Test]
        public void TestTurnOrdering()
        {
            var t1 = new BattleTeam(p1, FastUnit);
            var t2 = new BattleTeam(p2, SlowUnit);

            var map = new TacticsMap(32, 32);
            var battle = new TacticsBattle(map);

            battle.MoveUnitTo(t1.Units[0], 1, 1);
            battle.MoveUnitTo(t2.Units[0], 5, 5);

            Assert.AreEqual(battle.CurrentUnitTurn, t1.Units[0]);

            battle.PassTurn();

            Assert.AreEqual(battle.CurrentUnitTurn, t2.Units[0]);

            battle.PassTurn();

            Assert.AreEqual(battle.CurrentUnitTurn, t1.Units[0]);
        }

        [Test]
        public void TestRTCalculation()
        {
            var t1 = new BattleTeam(p1, FastUnit);
            var t2 = new BattleTeam(p2, SlowUnit);

            var map = new TacticsMap(32, 32);
            var battle = new TacticsBattle(map);

            battle.MoveUnitTo(t1.Units[0], 1, 1);
            battle.MoveUnitTo(t2.Units[0], 5, 5);

            Assert.AreEqual(battle.CurrentUnitTurn, t1.Units[0]);
            Assert.AreEqual(battle.CurrentUnitTurn.RT, battle.CurrentUnitTurn.GetMaxRT());

            battle.PassTurn();

            Assert.AreEqual(battle.CurrentUnitTurn, t2.Units[0]);
            Assert.AreEqual(battle.CurrentUnitTurn.RT, battle.CurrentUnitTurn.GetMaxRT());
            Assert.AreEqual(t1.Units[0].RT, 2 * t1.Units[0].GetMaxRT());
        }

        [Test]
        public void TestActionIncreasingRT()
        {
            var t1 = new BattleTeam(p1, FastUnit);
            var t2 = new BattleTeam(p2, SlowUnit);

            var map = new TacticsMap(32, 32);
            var battle = new TacticsBattle(map);

            battle.MoveUnitTo(t1.Units[0], 1, 1);
            battle.MoveUnitTo(t2.Units[0], 5, 5);

            Assert.AreEqual(battle.CurrentUnitTurn, t1.Units[0]);
            Assert.AreEqual(battle.CurrentUnitTurn.RT, battle.CurrentUnitTurn.GetMaxRT());

            var result = battle.ReceiveIntent(new MoveAction(t1.Units[0], 2, 2));
            Assert.AreEqual(true, result);
            Assert.Greater(battle.CurrentUnitTurn.RT, battle.CurrentUnitTurn.GetMaxRT());
        }
        */
    }
}