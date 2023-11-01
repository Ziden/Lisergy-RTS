using System;
using System.Runtime.InteropServices;

namespace Game.World
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct TileVector
    {
        public ushort X;
        public ushort Y;

        public TileVector(in ushort x, in ushort y)
        {
            X = x;
            Y = y;
        }

        public TileVector(in int x, in int y)
        {
            X = (ushort)x;
            Y = (ushort)y;
        }

        public override bool Equals(object obj)
        {
            return obj is TileVector position &&
                   X == position.X &&
                   Y == position.Y;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }

        public override string ToString()
        {
            return $"<Position X={X} Y={Y}>";
        }

        public static bool operator ==(in TileVector p1, in TileVector p2)
        {
            return p1.X == p2.X && p1.Y == p2.Y;
        }

        public static bool operator !=(in TileVector p1, in TileVector p2)
        {
            return p1.X != p2.X || p1.Y != p2.Y;
        }
    }
}
