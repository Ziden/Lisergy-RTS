﻿using Game.Engine.DataTypes;
using System;
using System.Text;

namespace Game.Engine.Network
{
    public class GameStream
    {
        public delegate void ArrayElementSerializer<T>(ref T element);

        private int _ptr;
        private int _maxPtr;
        private int _offsetBytes;
        private int _capacityBytes;
        private byte[] _data;
        private bool _write;

        public int Position
        {
            get => _ptr - (_offsetBytes << 3);
            set => _ptr = (_offsetBytes << 3) + BitUtils.Clamp(value, 0, _maxPtr - (_offsetBytes << 3));
        }

        public int BytesRequired => BitUtils.BytesRequired(Position);
        public bool IsEvenBytes => _ptr % 8 == 0;
        public int Capacity => _capacityBytes;
        public int Offset => _offsetBytes;
        public bool Done => _ptr == _maxPtr;
        public bool Overflowing => _ptr > _maxPtr;
        public bool Writing
        {
            get => _write;
            set => _write = value;
        }
        public bool Reading
        {
            get => !_write;
            set => _write = !value;
        }
        public byte[] Data => _data;

        public GameStream() : this(new byte[0]) { }

        public GameStream(int size) : this(new byte[size]) { }

        public GameStream(byte[] arr) : this(arr, arr.Length, 0) { }

        public GameStream(byte[] arr, int size) : this(arr, size, 0) { }

        public GameStream(byte[] arr, int size, int offset)
        {
            _data = arr;
            _ptr = offset << 3;
            _maxPtr = (offset + size) << 3;
            _offsetBytes = offset;
            _capacityBytes = size;
        }

        public void SetBuffer(byte[] arr) => SetBuffer(arr, arr.Length, 0);

        public void SetBuffer(byte[] arr, int size) => SetBuffer(arr, size, 0);

        public void SetBuffer(byte[] arr, int size, int offset)
        {
            _data = arr;
            _ptr = offset << 3;
            _maxPtr = (offset + size) << 3;
            _offsetBytes = offset;
            _capacityBytes = size;
        }

        public int RoundToByte()
        {
            int num = _ptr % 8;
            if (num > 0)
            {
                int num2 = 8 - num;
                if (_write)
                {
                    WriteByte(0, num2);
                }
                else
                {
                    _ptr += num2;
                }
            }
            return _ptr / 8;
        }

        public bool CanWrite() => CanWrite(1);

        public bool CanRead() => CanRead(1);

        public bool CanWrite(int bits) => _ptr + bits <= _maxPtr;

        public bool CanRead(int bits) => _ptr + bits <= _maxPtr;

        public void CopyFromArray(byte[] array)
        {
            Array.Copy(array, 0, _data, _offsetBytes, array.Length);
            _ptr = _offsetBytes << 3;
            _maxPtr = (_offsetBytes + array.Length) << 3;
        }

        public void Reset() => Reset(Capacity);

        public void Reset(int byteSize)
        {
            Array.Clear(_data, _offsetBytes, Capacity);
            _ptr = _offsetBytes << 3;
            _maxPtr = (_offsetBytes + byteSize) << 3;
        }

        public void ResetFast(int byteSize)
        {
            _ptr = _offsetBytes << 3;
            _maxPtr = (_offsetBytes + byteSize) << 3;
        }

        public byte[] ToArray()
        {
            byte[] array = new byte[BytesRequired];
            Buffer.BlockCopy(_data, _offsetBytes, array, 0, array.Length);
            return array;
        }

        public bool WriteBool(bool value)
        {
            InternalWriteByte((byte)(value ? 1 : 0), 1);
            return value;
        }

        public bool ReadBool() => InternalReadByte(1) == 1;

        public void WriteByte(byte value, int bits) => InternalWriteByte(value, bits);

        public byte ReadByte(int bits) => InternalReadByte(bits);

        public void WriteByte(byte value) => WriteByte(value, 8);

        public byte ReadByte() => ReadByte(8);

        public sbyte ReadSByte() => (sbyte)ReadByte();

        public void WriteSByte(sbyte value) => WriteByte((byte)value);

        public void WriteUShort(ushort value, int bits)
        {
            if (bits <= 8)
            {
                InternalWriteByte((byte)(value & 0xFFu), bits);
                return;
            }
            InternalWriteByte((byte)(value & 0xFFu), 8);
            InternalWriteByte((byte)(value >> 8), bits - 8);
        }

        public ushort ReadUShort(int bits)
        {
            if (bits <= 8)
            {
                return InternalReadByte(bits);
            }
            return (ushort)(InternalReadByte(8) | InternalReadByte(bits - 8) << 8);
        }

        public void WriteUShort(ushort value) => WriteUShort(value, 16);

        public ushort ReadUShort() => ReadUShort(16);

        public void WriteShort(short value, int bits) => WriteUShort((ushort)value, bits);

        public short ReadShort(int bits) => (short)ReadUShort(bits);

        public void WriteShort(short value) => WriteShort(value, 16);

        public short ReadShort() => ReadShort(16);

        public void WriteChar(char value) => WriteUShort(value, 16);

        public char ReadChar() => (char)ReadUShort(16);

        public void WriteUInt(uint value, int bits)
        {
            byte value2 = (byte)value;
            byte value3 = (byte)(value >> 8);
            byte value4 = (byte)(value >> 16);
            byte value5 = (byte)(value >> 24);
            switch ((bits + 7) / 8)
            {
                case 1:
                    InternalWriteByte(value2, bits);
                    break;
                case 2:
                    InternalWriteByte(value2, 8);
                    InternalWriteByte(value3, bits - 8);
                    break;
                case 3:
                    InternalWriteByte(value2, 8);
                    InternalWriteByte(value3, 8);
                    InternalWriteByte(value4, bits - 16);
                    break;
                case 4:
                    InternalWriteByte(value2, 8);
                    InternalWriteByte(value3, 8);
                    InternalWriteByte(value4, 8);
                    InternalWriteByte(value5, bits - 24);
                    break;
            }
        }

        public uint ReadUInt(int bits)
        {
            int num = 0;
            int num2 = 0;
            int num3 = 0;
            int num4 = 0;
            switch ((bits + 7) / 8)
            {
                case 1:
                    num = InternalReadByte(bits);
                    break;
                case 2:
                    num = InternalReadByte(8);
                    num2 = InternalReadByte(bits - 8);
                    break;
                case 3:
                    num = InternalReadByte(8);
                    num2 = InternalReadByte(8);
                    num3 = InternalReadByte(bits - 16);
                    break;
                case 4:
                    num = InternalReadByte(8);
                    num2 = InternalReadByte(8);
                    num3 = InternalReadByte(8);
                    num4 = InternalReadByte(bits - 24);
                    break;
            }
            return (uint)(num | num2 << 8 | num3 << 16 | num4 << 24);
        }

        public void WriteUInt(uint value) => WriteUInt(value, 32);

        public uint ReadUInt() => ReadUInt(32);

        public void WriteInt(int value, int bits) => WriteUInt((uint)value, bits);

        public int ReadInt(int bits) => (int)ReadUInt(bits);

        public void WriteInt(int value) => WriteInt(value, 32);

        public int ReadInt() => ReadInt(32);

        public void WriteULong(ulong value, int bits)
        {
            if (bits <= 32)
            {
                WriteUInt((uint)(value & 0xFFFFFFFFu), bits);
                return;
            }
            WriteUInt((uint)value, 32);
            WriteUInt((uint)(value >> 32), bits - 32);
        }

        public ulong ReadULong(int bits)
        {
            if (bits <= 32)
            {
                return ReadUInt(bits);
            }
            ulong num = ReadUInt(32);
            ulong num2 = ReadUInt(bits - 32);
            return num | num2 << 32;
        }

        public void WriteULong(ulong value) => WriteULong(value, 64);

        public ulong ReadULong() => ReadULong(64);

        public void WriteLong(long value, int bits) => WriteULong((ulong)value, bits);

        public long ReadLong(int bits) => (long)ReadULong(bits);

        public void WriteLong(long value) => WriteLong(value, 64);

        public long ReadLong() => ReadLong(64);

        public void WriteFloat(float value)
        {
            UdpByteConverter udpByteConverter = value;
            InternalWriteByte(udpByteConverter.Byte0, 8);
            InternalWriteByte(udpByteConverter.Byte1, 8);
            InternalWriteByte(udpByteConverter.Byte2, 8);
            InternalWriteByte(udpByteConverter.Byte3, 8);
        }

        public float ReadFloat()
        {
            UdpByteConverter udpByteConverter = default;
            udpByteConverter.Byte0 = InternalReadByte(8);
            udpByteConverter.Byte1 = InternalReadByte(8);
            udpByteConverter.Byte2 = InternalReadByte(8);
            udpByteConverter.Byte3 = InternalReadByte(8);
            return udpByteConverter.Float32;
        }

        public void WriteDouble(double value)
        {
            UdpByteConverter udpByteConverter = value;
            InternalWriteByte(udpByteConverter.Byte0, 8);
            InternalWriteByte(udpByteConverter.Byte1, 8);
            InternalWriteByte(udpByteConverter.Byte2, 8);
            InternalWriteByte(udpByteConverter.Byte3, 8);
            InternalWriteByte(udpByteConverter.Byte4, 8);
            InternalWriteByte(udpByteConverter.Byte5, 8);
            InternalWriteByte(udpByteConverter.Byte6, 8);
            InternalWriteByte(udpByteConverter.Byte7, 8);
        }

        public double ReadDouble()
        {
            UdpByteConverter udpByteConverter = default;
            udpByteConverter.Byte0 = InternalReadByte(8);
            udpByteConverter.Byte1 = InternalReadByte(8);
            udpByteConverter.Byte2 = InternalReadByte(8);
            udpByteConverter.Byte3 = InternalReadByte(8);
            udpByteConverter.Byte4 = InternalReadByte(8);
            udpByteConverter.Byte5 = InternalReadByte(8);
            udpByteConverter.Byte6 = InternalReadByte(8);
            udpByteConverter.Byte7 = InternalReadByte(8);
            return udpByteConverter.Float64;
        }

        public void WriteByteArray(byte[] from) => WriteByteArray(from, 0, from.Length);

        public void WriteByteArray(byte[] from, int count) => WriteByteArray(from, 0, count);

        public void WriteByteArray(byte[] from, int offset, int count)
        {
            int num = _ptr >> 3;
            int num2 = _ptr % 8;
            int num3 = 8 - num2;
            if (num2 == 0)
            {
                Buffer.BlockCopy(from, offset, _data, num, count);
            }
            else
            {
                for (int i = 0; i < count; i++)
                {
                    byte b = from[offset + i];
                    _data[num] &= (byte)(255 >> num3);
                    _data[num] |= (byte)(b << num2);
                    num++;
                    _data[num] &= (byte)(255 << num2);
                    _data[num] |= (byte)(b >> num3);
                }
            }
            _ptr += count * 8;
        }

        public byte[] ReadByteArray(int size)
        {
            byte[] array = new byte[size];
            ReadByteArray(array);
            return array;
        }

        public void ReadByteArray(byte[] to) => ReadByteArray(to, 0, to.Length);

        public void ReadByteArray(byte[] to, int count) => ReadByteArray(to, 0, count);

        public void ReadByteArray(byte[] to, int offset, int count)
        {
            int num = _ptr >> 3;
            int num2 = _ptr % 8;
            if (num2 == 0)
            {
                Buffer.BlockCopy(_data, num, to, offset, count);
            }
            else
            {
                int num3 = 8 - num2;
                for (int i = 0; i < count; i++)
                {
                    int num4 = _data[num] >> num2;
                    num++;
                    int num5 = _data[num] & 255 >> num3;
                    to[offset + i] = (byte)(num4 | num5 << num3);
                }
            }
            _ptr += count * 8;
        }

        public void WriteByteArrayLengthPrefixed(byte[] array) => WriteByteArrayLengthPrefixed(array, array?.Length ?? 0);

        public void WriteByteArrayLengthPrefixed(byte[] array, int maxLength)
        {
            if (WriteBool(array != null))
            {
                int num = Math.Min(array.Length, maxLength);
                if (num < array.Length)
                {
                    Console.WriteLine("Only sending {0}/{1} bytes from byte array", num, array.Length);
                }
                WriteUShort((ushort)num);
                WriteByteArray(array, 0, num);
            }
        }

        public byte[] ReadByteArrayLengthPrefixed()
        {
            if (ReadBool())
            {
                byte[] array = new byte[ReadUShort()];
                ReadByteArray(array, 0, array.Length);
                return array;
            }
            return null;
        }

        public void WriteString(string value, Encoding encoding)
        {
            if (!WriteBool(value == null))
            {
                byte[] bytes = encoding.GetBytes(value);
                WriteUShort((ushort)bytes.Length);
                WriteByteArray(bytes);
            }
        }

        public void WriteString(string value) => WriteString(value, Encoding.UTF8);

        public string ReadString(Encoding encoding)
        {
            if (ReadBool())
            {
                return null;
            }
            int num = ReadUShort();
            if (num == 0)
            {
                return "";
            }
            byte[] array = new byte[num];
            ReadByteArray(array);
            return encoding.GetString(array, 0, array.Length);
        }

        public string ReadString() => ReadString(Encoding.UTF8);

        public void WriteStringGZip(string value, Encoding encoding)
        {
            if (!WriteBool(value == null))
            {
                byte[] array = ByteUtils.GZipCompressString(value, encoding);
                WriteUShort((ushort)array.Length);
                WriteByteArray(array);
            }
        }

        public string ReadStringGZip(Encoding encoding)
        {
            if (ReadBool())
            {
                return null;
            }
            ushort num = ReadUShort();
            if (num == 0)
            {
                return "";
            }
            byte[] array = new byte[num];
            ReadByteArray(array);
            return ByteUtils.GZipDecompressString(array, encoding);
        }

        public void WriteGuid(Guid guid) => WriteByteArray(guid.ToByteArray());

        public Guid ReadGuid()
        {
            byte[] array = new byte[16];
            ReadByteArray(array);
            return new Guid(array);
        }

        private void InternalWriteByte(byte value, int bits)
        {
            WriteByteAt(_data, _ptr, bits, value);
            _ptr += bits;
        }

        public void WriteFP(FP fp) => WriteLong(fp.RawValue);

        public FP ReadFP() => FP.FromRaw(ReadLong());

        public void WriteNullableFP(FP fp) => WriteLong(fp.RawValue);

        public static void WriteByteAt(byte[] data, int ptr, int bits, byte value)
        {
            if (bits > 0)
            {
                value = (byte)(value & 255 >> 8 - bits);
                int num = ptr >> 3;
                int num2 = ptr & 7;
                int num3 = 8 - num2;
                int num4 = num3 - bits;
                if (num4 >= 0)
                {
                    int num5 = 255 >> num3 | 255 << 8 - num4;
                    data[num] = (byte)(data[num] & num5 | value << num2);
                }
                else
                {
                    data[num] = (byte)(data[num] & 255 >> num3 | value << num2);
                    data[num + 1] = (byte)(data[num + 1] & 255 << bits - num3 | value >> num3);
                }
            }
        }

        private byte InternalReadByte(int bits)
        {
            if (bits <= 0)
            {
                return 0;
            }
            int num = _ptr >> 3;
            int num2 = _ptr % 8;
            byte result;
            if (num2 == 0 && bits == 8)
            {
                result = _data[num];
            }
            else
            {
                int num3 = _data[num] >> num2;
                int num4 = bits - (8 - num2);
                if (num4 < 1)
                {
                    result = (byte)(num3 & 255 >> 8 - bits);
                }
                else
                {
                    int num5 = _data[num + 1] & 255 >> 8 - num4;
                    result = (byte)(num3 | num5 << bits - num4);
                }
            }
            _ptr += bits;
            return result;
        }
    }
}