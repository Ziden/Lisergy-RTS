using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Game.Engine.DataTypes
{
    /// <summary>
    /// Fixed implementation of a string
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    [Serializable]
    public unsafe struct FixedString : IEquatable<FixedString>
    {
        private static readonly Encoding ENCODING = Encoding.UTF8;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] _bytes;

        public static implicit operator FixedString(string s)
        {
            return new FixedString() { _bytes = ENCODING.GetBytes(s) };
        }

        public static implicit operator string(FixedString id)
        {
            return ENCODING.GetString(id._bytes);
        }

        public static bool operator ==(FixedString g1, FixedString g2)
        {
            return g1.IsEqualsTo(g2);
        }

        public static bool operator !=(FixedString g1, FixedString g2)
        {
            return !g1.IsEqualsTo(g2);
        }

        public bool Equals(FixedString obj)
        {
            return IsEqualsTo(obj);
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override string ToString()
        {
            return ENCODING.GetString(_bytes);
        }

        [DllImport("msvcrt.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int memcmp(IntPtr a1, IntPtr a2, uint count);

        public unsafe bool IsEqualsTo(FixedString str2)
        {
            fixed (byte* p1 = _bytes, p2 = str2._bytes)
            {
                return memcmp((IntPtr)p1, (IntPtr)p2, (uint)str2._bytes.Length) == 0;
            }
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_bytes);
        }
    }
}
