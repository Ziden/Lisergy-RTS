using System;
using Game.Engine.ECLS;
using Game.Systems.Battler;
using GameData.Specs;

namespace Game.Systems.Dungeon
{
	[Serializable]
	[SyncedComponent]
	[RequiresComponent(typeof(BattleGroupComponent))]
	public class DungeonComponent : IComponent
	{
		public DungeonSpecId SpecId;

		public override string ToString()
		{
			return $"<DungeonComponent Spec={SpecId}>";
		}
	}
}