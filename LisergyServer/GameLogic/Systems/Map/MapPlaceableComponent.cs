using System;
using Game.Engine.ECLS;

namespace Game.Systems.Map
{
    /// <summary>
    ///     Indicates an entity can be placed
    /// </summary>
    [Serializable]
	public class MapPlaceableComponent : IComponent
	{
		public override string ToString()
		{
			return "<MapPlaceable>";
		}
	}
}