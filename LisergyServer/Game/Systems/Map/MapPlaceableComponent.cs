using Game.Engine.ECLS;
using System;

namespace Game.Systems.Map
{
    /// <summary>
    /// Indicates an entity can be placed
    /// </summary>
    [Serializable]
    public class MapPlaceableComponent : IComponent
    {
        public override string ToString() => $"<MapPlaceable>";
    }
}
