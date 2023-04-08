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

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] _bytes;

        public static GameId Generate()
        {
            return new GameId() { _bytes = Guid.NewGuid().ToByteArray() };
        }

        public static implicit operator GameId(Guid id)
        {
            return new GameId()
            {
                _bytes = id == Guid.Empty ? new byte[16] : id.ToByteArray()
            };
        }

        public bool IsZero()
        {
            return _bytes == null || this == ZERO;
        }

        public static implicit operator Guid(GameId id)
        {
            return new Guid(id._bytes);
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
            if (id2._bytes == null)
            {
                return this == ZERO;
            }

            if (_bytes == null)
            {
                return id2 == ZERO;
            }

            unchecked
            {
                fixed (byte* p1 = _bytes, p2 = id2._bytes)
                {
                    return *(long*)p1 == *(long*)p2 && *(long*)p1 + 8 == *(long*)p2 + 8;
                }
            }
        }

        public GameId(MapPosition pos)
        {
            _bytes = new byte[16];
            fixed (byte* p1 = _bytes)
            {
                *(long*)p1 = pos.X;
                *(long*)(p1 + 1) = pos.Y;
            }
        }

        public override unsafe int GetHashCode()
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
