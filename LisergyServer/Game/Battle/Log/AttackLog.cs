using System;
using System.Collections.Generic;
using System.Text;

namespace Game.Battle.Log
{
    [Serializable]
    public class AttackLog : ActionLog
    {
        private string _defenderID;
        public ushort Damage;

        [NonSerialized]
        private BattleUnit _defender;

        public BattleUnit Defender { get => _defender; set  { _defender = value; _defenderID = value.Unit.Id; }  }

        public AttackLog(BattleUnit atk, BattleUnit def, int damage): base(atk)
        {
            this.Defender = def;
            this.Damage = (ushort)damage;
        }

        public override string ToString()
        {
            return $"<Attack {Attacker} to {Defender} damage={Damage}>";
        }
    }
}
