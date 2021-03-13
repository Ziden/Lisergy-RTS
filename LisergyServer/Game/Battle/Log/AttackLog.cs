using System;
using System.Collections.Generic;
using System.Text;

namespace Game.Battle.Log
{
    public class AttackLog : ActionLog
    {
        public BattleUnit Defender;
        public ushort Damage;

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
