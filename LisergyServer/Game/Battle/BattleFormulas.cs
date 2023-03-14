using Game.Battles.Actions;
using Game.BattleTactics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Game.Battles
{
    public static class BattleFormulas
    {
        public static AttackActionResult Attack(this BattleUnit attacker, BattleUnit defender)
        {
            var damage = attacker.Stats.Atk - (defender.Stats.Def / 2);
            var hp = defender.Stats.HP - damage;
            defender.Stats.HP = (ushort)Math.Max(0, hp);
            return new AttackActionResult() { Damage = damage };
        }
    }
}
