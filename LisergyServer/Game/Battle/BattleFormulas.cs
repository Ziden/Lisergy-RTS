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
            var damage = attacker.UnitReference.Atk - (defender.UnitReference.Def / 2);
            var hp = defender.UnitReference.HP - damage;
            defender.UnitReference.HP = (ushort)Math.Max(0, hp);
            return new AttackActionResult() { Damage = damage };
        }
    }
}
