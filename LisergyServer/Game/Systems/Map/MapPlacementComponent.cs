using Game.ECS;
using Game.World;
using System;

namespace Game.Systems.Map
{
    [Serializable]
    [SyncedComponent]
    public struct MapPlacementComponent : IComponent
    {
        public Position Position;

        public override string ToString() => $"<MapPlacementComponent {Position}>";
    }
}
