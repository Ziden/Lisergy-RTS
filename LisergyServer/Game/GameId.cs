using System;
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
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] _bytes;

        public static GameId Generate()
        {
            return new GameId() { _bytes = Guid.NewGuid().ToByteArray() };
        }

        public static implicit operator GameId(Guid id)
        {
            return new GameId() { _bytes = id.ToByteArray() };
        }

        public static implicit operator Guid(GameId id)
        {
            return new Guid(id._bytes);
        }

        public static implicit operator GameId(string id)
        {
            return new GameId() { _bytes = Guid.Parse(id).ToByteArray() };
        }

        public static implicit operator string(GameId id)
        {
            return new Guid(id._bytes).ToString();
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

        public override string ToString()
        {
            return base.ToString();
        }

        public unsafe bool IsEqualsTo(GameId id2)
        {
            unchecked
            {
                fixed (byte* p1 = _bytes, p2 = id2._bytes)
                {
                    return *(long*)p1 == *(long*)p2 && *(long*)p1 + 8 == *(long*)p2 + 8;
                }
            }
        }

        public unsafe override int GetHashCode()
        {
            unchecked
            {
                fixed (byte* p1 = _bytes)
                {
                    int hash = 0;
                    hash ^= (*(long*)p1).GetHashCode();
                    hash ^= (*(long*)p1 + 8).GetHashCode();
                    return hash;
                }
            }
        }
    }
}
