using Game.Battles;
using System;
using System.Linq;

namespace ServerTests
{
    public class TestBattle : TurnBattle
    {
        public TestBattle(BattleTeam t1, BattleTeam t2) : base(Guid.NewGuid(), t1, t2) { }

        public BattleUnit NextUnitToAct => _actionQueue.First();

        public TurnBattleResult Log => Result;
    }
}
