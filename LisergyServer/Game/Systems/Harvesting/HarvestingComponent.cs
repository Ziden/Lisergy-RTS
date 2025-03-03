using Game.Engine.ECLS;
using Game.World;
using System;
using System.Runtime.InteropServices;

namespace Game.Systems.Resources
{
    /// <summary>
    /// Component to be placed on entities while they are harvesting
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    [SyncedComponent]
    public class HarvestingComponent : IComponent
    {
        public Location Tile;
        public long StartedAt;
    }
}