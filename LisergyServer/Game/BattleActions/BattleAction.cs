using Game.BattleTactics;
using System;

namespace Game.Battles
{
    [Serializable]
    public class BattleAction
    {
        private string _unitID;

        [NonSerialized]
        private BattleUnit _unit;

        [NonSerialized]
        private ActionResult _result;

        public BattleUnit Unit { get => _unit; set { _unit = value; _unitID = value.UnitID; } }

        public ActionResult Result { get => _result; set => _result = value; }

        public BattleAction(BattleUnit atk)
        {
            this.Unit = atk;
        }
    }
}
