using Game.Engine.DataTypes;
using Game.Systems.Battle;
using Game.Systems.Battle.Data;
using Game.Systems.Battler;
using System.Linq;

namespace ServerTests
{
    public class TestBattle : TurnBattle
    {
        public TestBattle(in BattleGroupData t1, in BattleGroupData t2) : base(GameId.Generate(), t1, t2) { }

        public BattleUnit NextUnitToAct => _actionQueue.First();

        public TurnBattleRecord Log => Record;

        public static ref Unit MakeOverpower(ref Unit u)
        {
            u.Stats.Atk = 200;
            u.Stats.Speed = 200;
            u.Stats.Def = 200;
            u.Stats.HP = 200;
            u.Stats.MaxHP = 200;
            return ref u;
        }
    }
}
