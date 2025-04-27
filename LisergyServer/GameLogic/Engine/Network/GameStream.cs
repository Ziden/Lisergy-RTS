using System;
using System.Text;
using Game.Engine.DataTypes;

namespace Game.Engine.Network
{
	public class GameStream
	{
		public delegate void ArrayElementSerializer<T>(ref T element);

		private int _maxPtr;

		private int _ptr;

		public GameStream() : this(new byte[0])
		{
		}

		public GameStream(int size) : this(new byte[size])
		{
		}

		public GameStream(byte[] arr) : this(arr, arr.Length, 0)
		{
		}

		public GameStream(byte[] arr, int size) : this(arr, size, 0)
		{
		}

		public GameStream(byte[] arr, int size, int offset)
		{
			Data = arr;
			_ptr = offset << 3;
			_maxPtr = (offset + size) << 3;
			Offset = offset;
			Capacity = size;
		}

		public int Position
		{
			get => _ptr - (Offset << 3);
			set => _ptr = (Offset << 3) + BitUtils.Clamp(value, 0, _maxPtr - (Offset << 3));
		}

		public int BytesRequired => BitUtils.BytesRequired(Position);
		public bool IsEvenBytes => _ptr % 8 == 0;
		public int Capacity { get; private set; }

		public int Offset { get; private set; }

		public bool Done => _ptr == _maxPtr;
		public bool Overflowing => _ptr > _maxPtr;

		public bool Writing { get; set; }

		public bool Reading
		{
			get => !Writing;
			set => Writing = !value;
		}

		public byte[] Data { get; private set; }

		public void SetBuffer(byte[] arr)
		{
			SetBuffer(arr, arr.Length, 0);
		}

		public void SetBuffer(byte[] arr, int size)
		{
			SetBuffer(arr, size, 0);
		}

		public void SetBuffer(byte[] arr, int size, int offset)
		{
			Data = arr;
			_ptr = offset << 3;
			_maxPtr = (offset + size) << 3;
			Offset = offset;
			Capacity = size;
		}

		public int RoundToByte()
		{
			var num = _ptr % 8;
			if (num > 0)
			{
				var num2 = 8 - num;
				if (Writing)
					WriteByte(0, num2);
				else
					_ptr += num2;
			}

			return _ptr / 8;
		}

		public bool CanWrite()
		{
			return CanWrite(1);
		}

		public bool CanRead()
		{
			return CanRead(1);
		}

		public bool CanWrite(int bits)
		{
			return _ptr + bits <= _maxPtr;
		}

		public bool CanRead(int bits)
		{
			return _ptr + bits <= _maxPtr;
		}

		public void CopyFromArray(byte[] array)
		{
			Array.Copy(array, 0, Data, Offset, array.Length);
			_ptr = Offset << 3;
			_maxPtr = (Offset + array.Length) << 3;
		}

		public void Reset()
		{
			Reset(Capacity);
		}

		public void Reset(int byteSize)
		{
			Array.Clear(Data, Offset, Capacity);
			_ptr = Offset << 3;
			_maxPtr = (Offset + byteSize) << 3;
		}

		public void ResetFast(int byteSize)
		{
			_ptr = Offset << 3;
			_maxPtr = (Offset + byteSize) << 3;
		}

		public byte[] ToArray()
		{
			var array = new byte[BytesRequired];
			Buffer.BlockCopy(Data, Offset, array, 0, array.Length);
			return array;
		}

		public bool WriteBool(bool value)
		{
			InternalWriteByte((byte) (value ? 1 : 0), 1);
			return value;
		}

		public bool ReadBool()
		{
			return InternalReadByte(1) == 1;
		}

		public void WriteByte(byte value, int bits)
		{
			InternalWriteByte(value, bits);
		}

		public byte ReadByte(int bits)
		{
			return InternalReadByte(bits);
		}

		public void WriteByte(byte value)
		{
			WriteByte(value, 8);
		}

		public byte ReadByte()
		{
			return ReadByte(8);
		}

		public sbyte ReadSByte()
		{
			return (sbyte) ReadByte();
		}

		public void WriteSByte(sbyte value)
		{
			WriteByte((byte) value);
		}

		public void WriteUShort(ushort value, int bits)
		{
			if (bits <= 8)
			{
				InternalWriteByte((byte) (value & 0xFFu), bits);
				return;
			}

			InternalWriteByte((byte) (value & 0xFFu), 8);
			InternalWriteByte((byte) (value >> 8), bits - 8);
		}

		public ushort ReadUShort(int bits)
		{
			if (bits <= 8) return InternalReadByte(bits);
			return (ushort) (InternalReadByte(8) | (InternalReadByte(bits - 8) << 8));
		}

		public void WriteUShort(ushort value)
		{
			WriteUShort(value, 16);
		}

		public ushort ReadUShort()
		{
			return ReadUShort(16);
		}

		public void WriteShort(short value, int bits)
		{
			WriteUShort((ushort) value, bits);
		}

		public short ReadShort(int bits)
		{
			return (short) ReadUShort(bits);
		}

		public void WriteShort(short value)
		{
			WriteShort(value, 16);
		}

		public short ReadShort()
		{
			return ReadShort(16);
		}

		public void WriteChar(char value)
		{
			WriteUShort(value, 16);
		}

		public char ReadChar()
		{
			return (char) ReadUShort(16);
		}

		public void WriteUInt(uint value, int bits)
		{
			var value2 = (byte) value;
			var value3 = (byte) (value >> 8);
			var value4 = (byte) (value >> 16);
			var value5 = (byte) (value >> 24);
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
			var num = 0;
			var num2 = 0;
			var num3 = 0;
			var num4 = 0;
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

			return (uint) (num | (num2 << 8) | (num3 << 16) | (num4 << 24));
		}

		public void WriteUInt(uint value)
		{
			WriteUInt(value, 32);
		}

		public uint ReadUInt()
		{
			return ReadUInt(32);
		}

		public void WriteInt(int value, int bits)
		{
			WriteUInt((uint) value, bits);
		}

		public int ReadInt(int bits)
		{
			return (int) ReadUInt(bits);
		}

		public void WriteInt(int value)
		{
			WriteInt(value, 32);
		}

		public int ReadInt()
		{
			return ReadInt(32);
		}

		public void WriteULong(ulong value, int bits)
		{
			if (bits <= 32)
			{
				WriteUInt((uint) (value & 0xFFFFFFFFu), bits);
				return;
			}

			WriteUInt((uint) value, 32);
			WriteUInt((uint) (value >> 32), bits - 32);
		}

		public ulong ReadULong(int bits)
		{
			if (bits <= 32) return ReadUInt(bits);
			ulong num = ReadUInt(32);
			ulong num2 = ReadUInt(bits - 32);
			return num | (num2 << 32);
		}

		public void WriteULong(ulong value)
		{
			WriteULong(value, 64);
		}

		public ulong ReadULong()
		{
			return ReadULong(64);
		}

		public void WriteLong(long value, int bits)
		{
			WriteULong((ulong) value, bits);
		}

		public long ReadLong(int bits)
		{
			return (long) ReadULong(bits);
		}

		public void WriteLong(long value)
		{
			WriteLong(value, 64);
		}

		public long ReadLong()
		{
			return ReadLong(64);
		}

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

		public void WriteByteArray(byte[] from)
		{
			WriteByteArray(from, 0, from.Length);
		}

		public void WriteByteArray(byte[] from, int count)
		{
			WriteByteArray(from, 0, count);
		}

		public void WriteByteArray(byte[] from, int offset, int count)
		{
			var num = _ptr >> 3;
			var num2 = _ptr % 8;
			var num3 = 8 - num2;
			if (num2 == 0)
				Buffer.BlockCopy(from, offset, Data, num, count);
			else
				for (var i = 0; i < count; i++)
				{
					var b = from[offset + i];
					Data[num] &= (byte) (255 >> num3);
					Data[num] |= (byte) (b << num2);
					num++;
					Data[num] &= (byte) (255 << num2);
					Data[num] |= (byte) (b >> num3);
				}

			_ptr += count * 8;
		}

		public byte[] ReadByteArray(int size)
		{
			var array = new byte[size];
			ReadByteArray(array);
			return array;
		}

		public void ReadByteArray(byte[] to)
		{
			ReadByteArray(to, 0, to.Length);
		}

		public void ReadByteArray(byte[] to, int count)
		{
			ReadByteArray(to, 0, count);
		}

		public void ReadByteArray(byte[] to, int offset, int count)
		{
			var num = _ptr >> 3;
			var num2 = _ptr % 8;
			if (num2 == 0)
			{
				Buffer.BlockCopy(Data, num, to, offset, count);
			}
			else
			{
				var num3 = 8 - num2;
				for (var i = 0; i < count; i++)
				{
					var num4 = Data[num] >> num2;
					num++;
					var num5 = Data[num] & (255 >> num3);
					to[offset + i] = (byte) (num4 | (num5 << num3));
				}
			}

			_ptr += count * 8;
		}

		public void WriteByteArrayLengthPrefixed(byte[] array)
		{
			WriteByteArrayLengthPrefixed(array, array?.Length ?? 0);
		}

		public void WriteByteArrayLengthPrefixed(byte[] array, int maxLength)
		{
			if (WriteBool(array != null))
			{
				var num = Math.Min(array.Length, maxLength);
				if (num < array.Length)
					Console.WriteLine("Only sending {0}/{1} bytes from byte array", num, array.Length);
				WriteUShort((ushort) num);
				WriteByteArray(array, 0, num);
			}
		}

		public byte[] ReadByteArrayLengthPrefixed()
		{
			if (ReadBool())
			{
				var array = new byte[ReadUShort()];
				ReadByteArray(array, 0, array.Length);
				return array;
			}

			return null;
		}

		public void WriteString(string value, Encoding encoding)
		{
			if (!WriteBool(value == null))
			{
				var bytes = encoding.GetBytes(value);
				WriteUShort((ushort) bytes.Length);
				WriteByteArray(bytes);
			}
		}

		public void WriteString(string value)
		{
			WriteString(value, Encoding.UTF8);
		}

		public string ReadString(Encoding encoding)
		{
			if (ReadBool()) return null;
			int num = ReadUShort();
			if (num == 0) return "";
			var array = new byte[num];
			ReadByteArray(array);
			return encoding.GetString(array, 0, array.Length);
		}

		public string ReadString()
		{
			return ReadString(Encoding.UTF8);
		}

		public void WriteStringGZip(string value, Encoding encoding)
		{
			if (!WriteBool(value == null))
			{
				var array = ByteUtils.GZipCompressString(value, encoding);
				WriteUShort((ushort) array.Length);
				WriteByteArray(array);
			}
		}

		public string ReadStringGZip(Encoding encoding)
		{
			if (ReadBool()) return null;
			var num = ReadUShort();
			if (num == 0) return "";
			var array = new byte[num];
			ReadByteArray(array);
			return ByteUtils.GZipDecompressString(array, encoding);
		}

		public void WriteGuid(Guid guid)
		{
			WriteByteArray(guid.ToByteArray());
		}

		public Guid ReadGuid()
		{
			var array = new byte[16];
			ReadByteArray(array);
			return new Guid(array);
		}

		private void InternalWriteByte(byte value, int bits)
		{
			WriteByteAt(Data, _ptr, bits, value);
			_ptr += bits;
		}

		public void WriteFP(FP fp)
		{
			WriteLong(fp.RawValue);
		}

		public FP ReadFP()
		{
			return FP.FromRaw(ReadLong());
		}

		public void WriteNullableFP(FP fp)
		{
			WriteLong(fp.RawValue);
		}

		public static void WriteByteAt(byte[] data, int ptr, int bits, byte value)
		{
			if (bits > 0)
			{
				value = (byte) (value & (255 >> (8 - bits)));
				var num = ptr >> 3;
				var num2 = ptr & 7;
				var num3 = 8 - num2;
				var num4 = num3 - bits;
				if (num4 >= 0)
				{
					var num5 = (255 >> num3) | (255 << (8 - num4));
					data[num] = (byte) ((data[num] & num5) | (value << num2));
				}
				else
				{
					data[num] = (byte) ((data[num] & (255 >> num3)) | (value << num2));
					data[num + 1] = (byte) ((data[num + 1] & (255 << (bits - num3))) | (value >> num3));
				}
			}
		}

		private byte InternalReadByte(int bits)
		{
			if (bits <= 0) return 0;
			var num = _ptr >> 3;
			var num2 = _ptr % 8;
			byte result;
			if (num2 == 0 && bits == 8)
			{
				result = Data[num];
			}
			else
			{
				var num3 = Data[num] >> num2;
				var num4 = bits - (8 - num2);
				if (num4 < 1)
				{
					result = (byte) (num3 & (255 >> (8 - bits)));
				}
				else
				{
					var num5 = Data[num + 1] & (255 >> (8 - num4));
					result = (byte) (num3 | (num5 << (bits - num4)));
				}
			}

			_ptr += bits;
			return result;
		}
	}
}