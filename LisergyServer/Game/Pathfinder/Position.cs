using System;
using System.Runtime.InteropServices;

namespace Game.Pathfinder
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct Position
    {
        public ushort X;
        public ushort Y;

        public Position(ushort x, ushort y)
        {
            X = x;
            Y = y;
        }

        public Position(int x, int y)
        {
            X = (ushort)x;
            Y = (ushort)y;
        }

        public override bool Equals(object obj)
        {
            return obj is Position position &&
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

        public static bool operator ==(Position p1, Position p2)
        {
            return p1.X == p2.X && p1.Y == p2.Y;
        }

        public static bool operator !=(Position p1, Position p2)
        {
            return p1.X != p2.X || p1.Y != p2.Y;
        }
    }
}
