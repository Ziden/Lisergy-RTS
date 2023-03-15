using Game.BattleTactics;
using System;

namespace Game.Battles
{
    [Serializable]
    public class BattleAction
    {
        public GameId UnitID;

        [NonSerialized]
        private BattleUnit _unit;

        [NonSerialized]
        public TurnBattle Battle;

        private ActionResult _result;

        public BattleUnit Unit
        {
            get
            {
                if (_unit == null)
                    _unit = Battle.FindBattleUnit(this.UnitID);
                return _unit;
            }
            set { _unit = value; UnitID = value.UnitID; }
        }

        public ActionResult Result { get => _result; set => _result = value; }

        public BattleAction(TurnBattle battle, BattleUnit atk)
        {
            this.Unit = atk;
            Battle = battle;
        }
    }
}
