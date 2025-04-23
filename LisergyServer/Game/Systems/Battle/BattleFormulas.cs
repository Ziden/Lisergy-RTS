using System;
using Game.Systems.Battle.BattleActions;
using Game.Systems.Battle.Data;

namespace Game.Systems.Battle
{
	public static class BattleFormulas
	{
		public static AttackActionResult Attack(this BattleUnit attacker, BattleUnit defender)
		{
			var damage = attacker.UnitData.Stats.Atk - defender.UnitData.Stats.Def / 2;
			if (damage < 0) damage = 0;
			var hp = defender.UnitData.Stats.HP - damage;
			defender.UnitData.Stats.HP = (byte) Math.Max(0, hp);
			return new AttackActionResult {Damage = (ushort) damage};
		}
	}
}