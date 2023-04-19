using Game.Battle;
using Game.Battler;
using System;
using System.Linq;

namespace ServerTests
{
    public class TestBattle : TurnBattle
    {
        public TestBattle(BattleTeam t1, BattleTeam t2) : base(Guid.NewGuid(), t1, t2) { }

        public BattleUnit NextUnitToAct => _actionQueue.First();

        public TurnBattleRecord Log => Result;

        public static Unit MakeOverpower(Unit u)
        {
            u.Atk = 200;
            u.Speed = 200;
            u.Def = 200;
            u.HP = 200;
            u.MaxHP = 200;
            return u;
        }
    }
}
