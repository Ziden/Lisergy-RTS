using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Game.Engine.DataTypes
{
    [StructLayout(LayoutKind.Explicit)]
    internal struct UdpByteConverter
    {
        [FieldOffset(0)]
        public short Signed16;

        [FieldOffset(0)]
        public ushort Unsigned16;

        [FieldOffset(0)]
        public char Char;

        [FieldOffset(0)]
        public int Signed32;

        [FieldOffset(0)]
        public uint Unsigned32;

        [FieldOffset(0)]
        public long Signed64;

        [FieldOffset(0)]
        public ulong Unsigned64;

        [FieldOffset(0)]
        public float Float32;

        [FieldOffset(0)]
        public double Float64;

        [FieldOffset(0)]
        public byte Byte0;

        [FieldOffset(1)]
        public byte Byte1;

        [FieldOffset(2)]
        public byte Byte2;

        [FieldOffset(3)]
        public byte Byte3;

        [FieldOffset(4)]
        public byte Byte4;

        [FieldOffset(5)]
        public byte Byte5;

        [FieldOffset(6)]
        public byte Byte6;

        [FieldOffset(7)]
        public byte Byte7;

        public static implicit operator UdpByteConverter(short val)
        {
            UdpByteConverter result = default;
            result.Signed16 = val;
            return result;
        }

        public static implicit operator UdpByteConverter(ushort val)
        {
            UdpByteConverter result = default;
            result.Unsigned16 = val;
            return result;
        }

        public static implicit operator UdpByteConverter(char val)
        {
            UdpByteConverter result = default;
            result.Char = val;
            return result;
        }

        public static implicit operator UdpByteConverter(uint val)
        {
            UdpByteConverter result = default;
            result.Unsigned32 = val;
            return result;
        }

        public static implicit operator UdpByteConverter(int val)
        {
            UdpByteConverter result = default;
            result.Signed32 = val;
            return result;
        }

        public static implicit operator UdpByteConverter(ulong val)
        {
            UdpByteConverter result = default;
            result.Unsigned64 = val;
            return result;
        }

        public static implicit operator UdpByteConverter(long val)
        {
            UdpByteConverter result = default;
            result.Signed64 = val;
            return result;
        }

        public static implicit operator UdpByteConverter(float val)
        {
            UdpByteConverter result = default;
            result.Float32 = val;
            return result;
        }

        public static implicit operator UdpByteConverter(double val)
        {
            UdpByteConverter result = default;
            result.Float64 = val;
            return result;
        }
    }
}
