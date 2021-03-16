using System;

namespace Game.Battles
{
    [Serializable]
    public class ActionLog
    {
        private string _unitID;

        [NonSerialized]
        private BattleUnit _attacker;

        public BattleUnit Attacker { get => _attacker; set { _attacker = value; _unitID = value.Unit.Id; } }

        public ActionLog(BattleUnit atk)
        {
            this.Attacker = atk;
        }
    }
}
