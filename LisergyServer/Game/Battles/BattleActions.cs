using Game.Battles.Log;
using System;
using System.Collections.Generic;
using System.Text;

namespace Game.Battles
{
    public static class BattleActions
    {
        public static AttackLog Attack(this BattleUnit attacker, BattleUnit defender)
        {
            var damage = attacker.Stats.Atk - (defender.Stats.Def / 2);
            var hp = defender.Stats.HP - damage;
            defender.Stats.HP = (ushort)Math.Max(0, hp);
            return new AttackLog(attacker, defender, damage);
        }
    }
}
