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
    [StructLayout(LayoutKind.Sequential)]
    [Serializable]
    public struct TileMapData
    {
        public byte TileId;
        public Position Position;

        public ushort X { get => Position.X; set => Position.X = value; }
        public ushort Y { get => Position.Y; set => Position.Y = value; }

        public override string ToString() => $"<TileData {X}-{Y} ID={TileId}>";
    }
}
