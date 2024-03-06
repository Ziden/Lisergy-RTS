using Game.ECS;
using GameData;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Game.Systems.Construction
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    [Serializable]
    [SyncedComponent]
    public struct ConstructionComponent : IComponent
    {
        public long StartedAt;
        public long EndsAt;
        public BuildingSpecId Building;
    }
}
