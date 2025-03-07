using Game.Systems.Battle.BattleActions;
using Game.Systems.Battle.Data;
using System;

namespace Game.Systems.Battle
{
    public unsafe static class BattleFormulas
    {
        public unsafe static AttackActionResult Attack(this BattleUnit attacker, BattleUnit defender)
        {
            int damage = attacker.UnitData.Stats.Atk - defender.UnitData.Stats.Def / 2;
            if (damage < 0) damage = 0;
            int hp = defender.UnitData.Stats.HP - damage;
            defender.UnitData.Stats.HP = (byte)Math.Max(0, hp);
            return new AttackActionResult() { Damage = (ushort)damage };
        }
    }
}
