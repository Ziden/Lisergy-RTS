using Game.Battles.Actions;
using Game.BattleTactics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Game.Battles
{
    public static class BattleActions
    {
        public static AttackAction Attack(this BattleUnit attacker, BattleUnit defender)
        {
            var damage = attacker.Stats.Atk - (defender.Stats.Def / 2);
            var hp = defender.Stats.HP - damage;
            defender.Stats.HP = (short)Math.Max(0, hp);
            var result = new AttackActionResult() { Damage = damage };
            var action = new AttackAction(attacker, defender);
            action.Result = result;
            return action;
        }
    }
}
