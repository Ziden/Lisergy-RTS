using Game.BattleActions;
using System;

namespace Game.Battle
{
    public static class BattleFormulas
    {
        public static AttackActionResult Attack(this BattleUnit attacker, BattleUnit defender)
        {
            int damage = attacker.UnitReference.Atk - (defender.UnitReference.Def / 2);
            int hp = defender.UnitReference.HP - damage;
            defender.UnitReference.HP = (ushort)Math.Max(0, hp);
            return new AttackActionResult() { Damage = damage };
        }
    }
}
