using Game.ECS;
using System;

namespace Game.Systems.Building
{
    [Serializable]
    [SyncedComponent]
    public struct PlayerBuildingComponent : IComponent
    {
        public ushort SpecId;
    }
}
