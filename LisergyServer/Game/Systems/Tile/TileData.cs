using Game.Pathfinder;
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
    public struct TileData
    {
        public byte TileId;

        public override string ToString() => $"<TileData ID={TileId}>";
    }
}
