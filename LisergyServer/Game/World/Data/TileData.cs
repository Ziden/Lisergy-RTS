using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Game.World.Data
{
    [StructLayout(LayoutKind.Sequential)]
    [Serializable]
    public struct TileData
    {
        public byte ResourceId;
        public byte TileId;
        public Position Position;

        public ushort X { get => Position.X; set => Position.X = value; }
        public ushort Y { get => Position.Y; set => Position.Y = value; }

        public override string ToString()
        {
            return $"<TileData {X}-{Y} ID={TileId}>";

        }
    }
}
