using Game.ECS;
using System;

namespace Game.Systems.Map
{
    [Serializable]
    [SyncedComponent]
    public class MapPositionComponent : IComponent
    {
        public ushort X;
        public ushort Y;

        public override string ToString()
        {
            return $"<MapPositionComponent X={X} Y={Y}>";
        }
    }
}
