using Game.Engine.ECLS;
using GameData;
using System;
using System.Runtime.InteropServices;

namespace Game.Systems.Construction
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    [Serializable]
    [SyncedComponent]
    public class ConstructionComponent : IComponent
    {
        public long StartedAt;
        public long EndsAt;
        public BuildingSpecId Building;
    }
}
