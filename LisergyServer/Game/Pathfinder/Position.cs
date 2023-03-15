using System;

namespace Game.World
{
    [Serializable]
    public struct Position
    {
        public ushort X;
        public ushort Y;

        public Position(ushort x, ushort y)
        {
            this.X = x;
            this.Y = y;
        }

        public Position(int x, int y)
        {
            this.X = (ushort)x;
            this.Y = (ushort)y;
        }

        public new string ToString()
        {
            return X + "_" + Y;
        }

        public static bool operator ==(Position p1, Position p2)
        {
            return (p1.X == p2.X && p1.Y == p2.Y);
        }

        public static bool operator !=(Position p1, Position p2)
        {
            return (p1.X != p2.X || p1.Y != p2.Y);
        }
    }
}
