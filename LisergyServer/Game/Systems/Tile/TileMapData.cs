using Game.Pathfinder;
using Game.Systems.World;
using System;
using System.Runtime.InteropServices;

namespace Game.Systems.Tile
{
    [StructLayout(LayoutKind.Sequential)]
    [Serializable]
    public struct TileMapData
    {
        public byte TileId;
        public MapPosition Position;

        public ushort X { get => Position.X; set => Position.X = value; }
        public ushort Y { get => Position.Y; set => Position.Y = value; }

        public override string ToString()
        {
            return $"<TileData {X}-{Y} ID={TileId}>";

        }
    }
}
