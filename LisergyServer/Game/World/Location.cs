using System;
using System.Runtime.InteropServices;

namespace Game.World
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct Location
    {
        public ushort X;
        public ushort Y;

        public Location(in ushort x, in ushort y)
        {
            X = x;
            Y = y;
        }

        public Location(in int x, in int y)
        {
            X = (ushort)x;
            Y = (ushort)y;
        }

        public override bool Equals(object obj)
        {
            return obj is Location position &&
                   X == position.X &&
                   Y == position.Y;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }

        public override string ToString()
        {
            return $"(X={X} Y={Y})";
        }

        public static bool operator ==(in Location p1, in Location p2)
        {
            return p1.X == p2.X && p1.Y == p2.Y;
        }

        public static bool operator !=(in Location p1, in Location p2)
        {
            return p1.X != p2.X || p1.Y != p2.Y;
        }
    }
}
