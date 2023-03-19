using Game.ECS;
using System;

namespace Game.World.Components
{
    [Serializable]
    [SyncedComponent]
    public class PlayerBuildingComponent : IComponent
    {
        public ushort SpecId;
    }
}
