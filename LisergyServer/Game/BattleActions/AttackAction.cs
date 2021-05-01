using System;
using System.Collections.Generic;
using System.Text;

namespace Game.Battles.Actions
{
    [Serializable]
    public class AttackAction : BattleAction
    {
        private string _defenderID;

        [NonSerialized]
        private BattleUnit _defender;

        public BattleUnit Defender { get => _defender; set  { _defender = value; _defenderID = value.UnitID; }  }

        public AttackAction(BattleUnit atk, BattleUnit def): base(atk)
        {
            this.Defender = def;
        }

        public override string ToString()
        {
            return $"<Attack {Unit} to {Defender}";
        }
    }
}
