using System;
using System.Runtime.InteropServices;

namespace Game.World
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct MapPosition
    {
        public ushort X;
        public ushort Y;

        public MapPosition(ushort x, ushort y)
        {
            X = x;
            Y = y;
        }

        public MapPosition(int x, int y)
        {
            X = (ushort)x;
            Y = (ushort)y;
        }

        public override bool Equals(object obj)
        {
            return obj is MapPosition position &&
                   X == position.X &&
                   Y == position.Y;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }

        public override string ToString()
        {
            return X + "_" + Y;
        }

        public static bool operator ==(MapPosition p1, MapPosition p2)
        {
            return p1.X == p2.X && p1.Y == p2.Y;
        }

        public static bool operator !=(MapPosition p1, MapPosition p2)
        {
            return p1.X != p2.X || p1.Y != p2.Y;
        }
    }
}
