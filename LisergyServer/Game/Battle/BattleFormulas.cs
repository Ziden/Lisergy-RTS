using Game.Battle.BattleActions;
using System;

namespace Game.Battle
{
    public static class BattleFormulas
    {
        public static AttackActionResult Attack(this BattleUnit attacker, BattleUnit defender)
        {
            int damage = attacker.UnitReference.Atk - defender.UnitReference.Def / 2;
            if (damage < 0) damage = 0;
            int hp = defender.UnitReference.HP - damage;
            var u = defender.UnitReference;
            u.HP = (byte)Math.Max(0, hp);
            defender.UnitReference = u;
            return new AttackActionResult() { Damage = (ushort)damage };
        }
    }
}
