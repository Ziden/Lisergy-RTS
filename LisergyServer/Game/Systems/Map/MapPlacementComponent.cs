using Game.ECS;
using Game.World;
using System;

namespace Game.Systems.Map
{
    /// <summary>
    /// Refers to an entity that is placed in the map
    /// </summary>
    [Serializable]
    [SyncedComponent]
    public struct MapPlacementComponent : IComponent
    {
        public Position Position;

        public override string ToString() => $"<MapPlacementComponent {Position}>";
    }
}
