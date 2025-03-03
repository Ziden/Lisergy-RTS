using Game.Engine.ECLS;
using System;
using System.Runtime.InteropServices;

namespace Game.Systems.Resources
{
    /// <summary>
    /// Represents a resource harvest point that can be harvested
    /// This component is added to tiles
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    [SyncedComponent]
    [NonPersisted]
    public class TileResourceComponent : IComponent
    {
        public ResourceStackData Resource;

        /// <summary>
        /// If this tile is currently being harvested or not
        /// </summary>
        public bool BeingHarvested;
    }
}