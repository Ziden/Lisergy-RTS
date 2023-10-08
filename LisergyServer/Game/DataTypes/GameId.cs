using Game.Pathfinder;
using Game.World;
using System;
using System.Runtime.InteropServices;

namespace Game.DataTypes
{
    /// <summary>
    /// Simpler to serialize structure of Guids.
    /// Guarantees uniqueness using Guids.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    [Serializable]
    public unsafe struct GameId : IEquatable<GameId>
    {
        public static GameId ZERO = Guid.Empty;

        private ulong leftside;
        private ulong rightside;

        public byte[] GetBytes() {
            var bytes = new byte[16];
            fixed (byte* pointer = bytes)
            {
                *(ulong*)pointer = leftside;
                *(ulong*)(pointer + 8) = rightside;
            }
            return bytes;
        }

        public static GameId Generate()
        {
            return Guid.NewGuid();
        }

        public static implicit operator GameId(Guid id)
        {
            var bytes = id.ToByteArray();
            fixed (byte* pointer = bytes)
            {
                return new GameId()
                {
                    leftside = *(ulong*)pointer,
                    rightside = *(ulong*)(pointer + 8)
                };
            }
        }

        public bool IsZero()
        {
            return this == ZERO;
        }

        public static implicit operator Guid(GameId id)
        {
            return new Guid(id.GetBytes());
        }


        public static bool operator !=(GameId g1, Guid g2)
        {
            return !g1.IsEqualsTo(g2);
        }

        public static bool operator ==(GameId g1, Guid g2)
        {
            return g1.IsEqualsTo(g2);
        }

        public static bool operator !=(GameId g1, GameId g2)
        {
            return !g1.IsEqualsTo(g2);
        }

        public static bool operator ==(GameId g1, GameId g2)
        {
            return g1.IsEqualsTo(g2);
        }

        public bool Equals(GameId obj)
        {
            return IsEqualsTo(obj);
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override string ToString()
        {
            if (IsZero())
            {
                return Guid.Empty.ToString();
            }

            Guid g = this;
            return g.ToString();
        }

        public unsafe bool IsEqualsTo(GameId id2)
        {
            return leftside == id2.leftside && rightside == id2.rightside;
        }

        public GameId(Position pos)
        {
            leftside = pos.X;
            rightside = pos.Y;
        }

        public GameId(ulong l, ulong l2)
        {
            leftside = l;
            rightside = l2;
        }

        public override unsafe int GetHashCode()
        {
            return HashCode.Combine(leftside, rightside);
        }
    }
}
