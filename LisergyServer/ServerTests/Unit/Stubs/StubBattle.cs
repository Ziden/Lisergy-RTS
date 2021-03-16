using Game.Battles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServerTests
{
    public class TestBattle : Battle
    {
        public TestBattle(BattleTeam t1, BattleTeam t2) : base(t1, t2) { }

        public ActionLog RunSingleTurn()
        {
            this.DoRound();
            return Log.LastAction;
        }

        public BattleUnit NextUnitToAct => _actionQueue.First();

        public BattleResult Log => _log;
    }
}
