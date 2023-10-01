using Game.ECS;
using System;

namespace Game.Systems.Building
{
    [Serializable]
    [SyncedComponent]
    public class PlayerBuildingComponent : IComponent
    {
        public ushort SpecId;
    }
}
