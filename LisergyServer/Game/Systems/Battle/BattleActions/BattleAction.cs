using Game.Engine.DataTypes;
using Game.Systems.Battle.Data;
using System;

namespace Game.Systems.Battle.BattleActions
{
    [Serializable]
    public class BattleAction : BattleEvent
    {
        public GameId UnitID;
        private ActionResult _result;

        [NonSerialized]
        private BattleUnit _unit;

        [NonSerialized]
        public TurnBattle Battle;

        public BattleUnit Unit
        {
            get
            {
                if (_unit == null)
                {
                    _unit = Battle.FindBattleUnit(UnitID);
                }

                return _unit;
            }
            set { _unit = value; UnitID = value.UnitID; }
        }

        public ActionResult Result { get => _result; set => _result = value; }

        public BattleAction(TurnBattle battle, BattleUnit atk)
        {
            Unit = atk;
            Battle = battle;
        }
    }
}
