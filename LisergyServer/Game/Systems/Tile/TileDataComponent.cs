using Game.Engine.ECLS;
using Game.World;
using System;
using System.Runtime.InteropServices;

namespace Game.Systems.Tile
{
    /// <summary>
    /// Main struct data used for tiles.
    /// This struct represents a single tile. It's size should be as low as possible.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    [Serializable]
    [SyncedComponent]
    public class TileDataComponent : IComponent
    {
        public byte TileId;
        public Location Position;

        public override string ToString() => $"({Position} Spec={TileId})";
    }
}
