using Game.World;
using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace Game
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

        private ulong l1;
        private ulong l2;

        public void FromPointer(ulong* pointer)
        {
            l1 = *pointer;
            l2 = *(pointer + 1);
        }

        public GameId(Guid g)
        {
            l1 = 0;
            l2 = 0;
            FromPointer((ulong*)&g);
        }

        public static GameId Generate()
        {
            return new GameId(Guid.NewGuid());
        }

        public GameId(Position pos)
        {
            l1 = 0;
            l2 = 0;
            FromPointer((ulong*)&pos);

        }

        public bool IsZero() => l1 == l2 && l2 == 0;

        public static implicit operator GameId(Guid id)
        {
            return new GameId(id);
        }

        public static bool operator ==(GameId g1, GameId g2)
        {
            return g1.IsEqualsTo(g2);
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

        public bool Equals(GameId obj) => this.IsEqualsTo(obj);

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public unsafe bool IsEqualsTo(GameId a1)
        {
            return l1 == a1.l1 && l2 == a1.l2;
        }

        public byte[] GetBytes()
        {
            var bytes = new byte[16];
            fixed (byte* pointer = bytes)
            {
                *((ulong*)pointer) = l1;
                *((ulong*)pointer + 1) = l2;
            }
            return bytes;
        }

        public unsafe override int GetHashCode()
        {
            int hash = 0;
            hash += l1.GetHashCode() * 31;
            hash += l2.GetHashCode() * 31;
            return hash;
        }

        public override string ToString()
        {
            return $"{l1}-{l2}";
        }
    }
}
