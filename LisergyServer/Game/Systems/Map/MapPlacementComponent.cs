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
	public class MapPlacementComponent : IComponent
	{
		public Location Position;

		public override string ToString()
		{
			return $"<MapPlacementComponent {Position}>";
		}
	}
}