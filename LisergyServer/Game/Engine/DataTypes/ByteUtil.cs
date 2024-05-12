using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.IO;
using System.Linq;
using System.Text;

namespace Game.Engine.DataTypes
{
    public class ByteUtils
    {
        public unsafe static byte[] ToByteArray(byte* ptr, int length)
        {
            byte[] array = new byte[length];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = ptr[i];
            }

            return array;
        }

        public static byte[] MergeByteBlocks(params byte[][] blocks)
        {
            int num = blocks.Select((x) => x.Length).Sum();
            byte[] array = new byte[num];
            int num2 = 0;
            for (int i = 0; i < blocks.Length; i++)
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
            return AddValueBlock((long)value, buffer, offset);
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
            int num = blocks.Select((x) => x.Length).Sum() + blocks.Length * 4;
            byte[] array = new byte[num];
            int num2 = 0;
            for (int i = 0; i < blocks.Length; i++)
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
            int dataOffset2 = 0;
            while (dataOffset2 < data.Length)
            {
                byte[] array = new byte[BitConverter.ToInt32(data, dataOffset2)];
                dataOffset2 += 4;
                Array.Copy(data, dataOffset2, array, 0, array.Length);
                dataOffset2 += array.Length;
                yield return array;
            }
        }

        public static string PrintBits(Array array, int offset, int length)
        {
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < length; i++)
            {
                byte @byte = Buffer.GetByte(array, offset + i);
                for (int j = 0; j < 8; j++)
                {
                    stringBuilder.Append((@byte & 1 << j) == 0 ? "0" : "1");
                }

                if (i + 1 != length)
                {
                    stringBuilder.Append(" ");
                }
            }

            return stringBuilder.ToString();
        }

        private static void CopyTo(Stream source, Stream destination)
        {
            byte[] array = new byte[4096];
            int num = 0;
            while ((num = source.Read(array, 0, array.Length)) != 0)
            {
                destination.Write(array, 0, num);
            }
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
            using MemoryStream memoryStream = new MemoryStream();
            GZipCompressBytes(data, 0, data.Length, memoryStream);
            return memoryStream.ToArray();
        }

        public static void GZipCompressBytes(byte[] data, int offset, int size, Stream output)
        {
            using MemoryStream input = new MemoryStream(data, offset, size);
            GZipCompressBytes(input, output);
        }

        public static void GZipCompressBytes(Stream input, Stream output)
        {
            using GZipStream destination = CreateGZipCompressStream(output);
            CopyTo(input, destination);
        }

        public static GZipStream CreateGZipCompressStream(Stream output)
        {
            return new GZipStream(output, CompressionMode.Compress, leaveOpen: true);
        }

        public static byte[] GZipDecompressBytes(byte[] data)
        {
            using MemoryStream stream = new MemoryStream(data);
            using MemoryStream memoryStream = new MemoryStream();
            using (GZipStream source = new GZipStream(stream, CompressionMode.Decompress))
            {
                CopyTo(source, memoryStream);
            }

            return memoryStream.ToArray();
        }

        public static byte[] GZipCompressString(string data, Encoding encoding)
        {
            using MemoryStream source = new MemoryStream(encoding.GetBytes(data));
            using MemoryStream memoryStream = new MemoryStream();
            using (GZipStream destination = new GZipStream(memoryStream, CompressionMode.Compress))
            {
                CopyTo(source, destination);
            }

            return memoryStream.ToArray();
        }

        public static string GZipDecompressString(byte[] data, Encoding encoding)
        {
            using MemoryStream stream = new MemoryStream(data);
            using MemoryStream memoryStream = new MemoryStream();
            using (GZipStream source = new GZipStream(stream, CompressionMode.Decompress))
            {
                CopyTo(source, memoryStream);
            }

            return encoding.GetString(memoryStream.ToArray());
        }

        public unsafe static int WriteBytes(long value, byte[] array, int offset)
        {
            fixed (byte* ptr = array)
            {
                byte* ptr2 = ptr + offset;
                *(long*)ptr2 = value;
                return 8;
            }
        }

        public static int WriteBytes(ulong value, byte[] array, int offset)
        {
            return WriteBytes((long)value, array, offset);
        }

        public unsafe static int WriteBytes(int value, byte[] array, int offset)
        {
            fixed (byte* ptr = array)
            {
                byte* ptr2 = ptr + offset;
                *(int*)ptr2 = value;
                return 4;
            }
        }
    }
}
