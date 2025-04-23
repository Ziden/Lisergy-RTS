using System;
using Game.Engine.ECLS;
using Game.World;

namespace Game.Systems.Map
{
    /// <summary>
    ///     Refers to an entity that is placed in the map
    /// </summary>
    [Serializable]
	[SyncedComponent]
	public class PreviousMapPlacementComponent : IComponent
	{
		public Location Position;

		public override string ToString()
		{
			return $"<PreviousMapPlacement {Position}>";
		}
	}
}