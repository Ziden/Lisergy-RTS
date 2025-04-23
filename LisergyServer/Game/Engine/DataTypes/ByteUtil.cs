using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace Game.Engine.DataTypes
{
	public class ByteUtils
	{
		public static unsafe byte[] ToByteArray(byte* ptr, int length)
		{
			var array = new byte[length];
			for (var i = 0; i < array.Length; i++) array[i] = ptr[i];

			return array;
		}

		public static byte[] MergeByteBlocks(params byte[][] blocks)
		{
			var num = blocks.Select(x => x.Length).Sum();
			var array = new byte[num];
			var num2 = 0;
			for (var i = 0; i < blocks.Length; i++)
			{
				Array.Copy(blocks[i], 0, array, num2, blocks[i].Length);
				num2 += blocks[i].Length;
			}

			return array;
		}

		public static int AddValueBlock(int value, byte[] buffer, int offset)
		{
			offset += WriteBytes(4, buffer, offset);
			offset += WriteBytes(value, buffer, offset);
			return offset;
		}

		public static int AddValueBlock(long value, byte[] buffer, int offset)
		{
			offset += WriteBytes(8, buffer, offset);
			offset += WriteBytes(value, buffer, offset);
			return offset;
		}

		public static int AddValueBlock(ulong value, byte[] buffer, int offset)
		{
			return AddValueBlock((long) value, buffer, offset);
		}

		public static int AddByteBlock(byte[] block, byte[] buffer, int offset)
		{
			Array.Copy(BitConverter.GetBytes(block.Length), 0, buffer, offset, 4);
			offset += 4;
			Array.Copy(block, 0, buffer, offset, block.Length);
			offset += block.Length;
			return offset;
		}

		public static int BeginByteBlockHeader(byte[] buffer, int offset, out int blockStart)
		{
			blockStart = offset;
			return offset += 4;
		}

		public static int EndByteBlockHeader(byte[] buffer, int blockStart, int bytesWritten)
		{
			return blockStart + WriteBytes(bytesWritten, buffer, blockStart) + bytesWritten;
		}

		public static byte[] PackByteBlocks(params byte[][] blocks)
		{
			var num = blocks.Select(x => x.Length).Sum() + blocks.Length * 4;
			var array = new byte[num];
			var num2 = 0;
			for (var i = 0; i < blocks.Length; i++)
			{
				Array.Copy(BitConverter.GetBytes(blocks[i].Length), 0, array, num2, 4);
				num2 += 4;
				Array.Copy(blocks[i], 0, array, num2, blocks[i].Length);
				num2 += blocks[i].Length;
			}

			return array;
		}

		public static IEnumerable<byte[]> ReadByteBlocks(byte[] data)
		{
			var dataOffset2 = 0;
			while (dataOffset2 < data.Length)
			{
				var array = new byte[BitConverter.ToInt32(data, dataOffset2)];
				dataOffset2 += 4;
				Array.Copy(data, dataOffset2, array, 0, array.Length);
				dataOffset2 += array.Length;
				yield return array;
			}
		}

		public static string PrintBits(Array array, int offset, int length)
		{
			var stringBuilder = new StringBuilder();
			for (var i = 0; i < length; i++)
			{
				var @byte = Buffer.GetByte(array, offset + i);
				for (var j = 0; j < 8; j++) stringBuilder.Append((@byte & (1 << j)) == 0 ? "0" : "1");

				if (i + 1 != length) stringBuilder.Append(" ");
			}

			return stringBuilder.ToString();
		}

		private static void CopyTo(Stream source, Stream destination)
		{
			var array = new byte[4096];
			var num = 0;
			while ((num = source.Read(array, 0, array.Length)) != 0) destination.Write(array, 0, num);
		}

		public static string Base64EncodeString(string data, Encoding encoding)
		{
			return Base64Encode(encoding.GetBytes(data));
		}

		public static string Base64DecodeString(string data, Encoding encoding)
		{
			return encoding.GetString(Base64Decode(data));
		}

		public static string Base64Encode(byte[] data)
		{
			return Convert.ToBase64String(data);
		}

		public static byte[] Base64Decode(string data)
		{
			return Convert.FromBase64String(data);
		}

		public static byte[] GZipCompressBytes(byte[] data)
		{
			using var memoryStream = new MemoryStream();
			GZipCompressBytes(data, 0, data.Length, memoryStream);
			return memoryStream.ToArray();
		}

		public static void GZipCompressBytes(byte[] data, int offset, int size, Stream output)
		{
			using var input = new MemoryStream(data, offset, size);
			GZipCompressBytes(input, output);
		}

		public static void GZipCompressBytes(Stream input, Stream output)
		{
			using var destination = CreateGZipCompressStream(output);
			CopyTo(input, destination);
		}

		public static GZipStream CreateGZipCompressStream(Stream output)
		{
			return new GZipStream(output, CompressionMode.Compress, true);
		}

		public static byte[] GZipDecompressBytes(byte[] data)
		{
			using var stream = new MemoryStream(data);
			using var memoryStream = new MemoryStream();
			using (var source = new GZipStream(stream, CompressionMode.Decompress))
			{
				CopyTo(source, memoryStream);
			}

			return memoryStream.ToArray();
		}

		public static byte[] GZipCompressString(string data, Encoding encoding)
		{
			using var source = new MemoryStream(encoding.GetBytes(data));
			using var memoryStream = new MemoryStream();
			using (var destination = new GZipStream(memoryStream, CompressionMode.Compress))
			{
				CopyTo(source, destination);
			}

			return memoryStream.ToArray();
		}

		public static string GZipDecompressString(byte[] data, Encoding encoding)
		{
			using var stream = new MemoryStream(data);
			using var memoryStream = new MemoryStream();
			using (var source = new GZipStream(stream, CompressionMode.Decompress))
			{
				CopyTo(source, memoryStream);
			}

			return encoding.GetString(memoryStream.ToArray());
		}

		public static unsafe int WriteBytes(long value, byte[] array, int offset)
		{
			fixed (byte* ptr = array)
			{
				var ptr2 = ptr + offset;
				*(long*) ptr2 = value;
				return 8;
			}
		}

		public static int WriteBytes(ulong value, byte[] array, int offset)
		{
			return WriteBytes((long) value, array, offset);
		}

		public static unsafe int WriteBytes(int value, byte[] array, int offset)
		{
			fixed (byte* ptr = array)
			{
				var ptr2 = ptr + offset;
				*(int*) ptr2 = value;
				return 4;
			}
		}
	}
}