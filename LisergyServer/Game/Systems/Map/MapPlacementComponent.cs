using Game.ECS;
using Game.World;
using System;

namespace Game.Systems.Map
{
    [Serializable]
    [SyncedComponent]
    public class MapPlacementComponent : IComponent
    {
        private Position _position;
        public ref Position Position => ref _position;
        public override string ToString()
        {
            return $"<MapPositionComponent {_position}>";
        }
    }
}
