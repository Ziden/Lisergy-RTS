using System.Text;

internal static class BitUtils
{
    private static readonly string[] _byteHexValue = new string[256]
    {
            "00", "01", "02", "03", "04", "05", "06", "07", "08", "09",
            "0A", "0B", "0C", "0D", "0E", "0F", "10", "11", "12", "13",
            "14", "15", "16", "17", "18", "19", "1A", "1B", "1C", "1D",
            "1E", "1F", "20", "21", "22", "23", "24", "25", "26", "27",
            "28", "29", "2A", "2B", "2C", "2D", "2E", "2F", "30", "31",
            "32", "33", "34", "35", "36", "37", "38", "39", "3A", "3B",
            "3C", "3D", "3E", "3F", "40", "41", "42", "43", "44", "45",
            "46", "47", "48", "49", "4A", "4B", "4C", "4D", "4E", "4F",
            "50", "51", "52", "53", "54", "55", "56", "57", "58", "59",
            "5A", "5B", "5C", "5D", "5E", "5F", "60", "61", "62", "63",
            "64", "65", "66", "67", "68", "69", "6A", "6B", "6C", "6D",
            "6E", "6F", "70", "71", "72", "73", "74", "75", "76", "77",
            "78", "79", "7A", "7B", "7C", "7D", "7E", "7F", "80", "81",
            "82", "83", "84", "85", "86", "87", "88", "89", "8A", "8B",
            "8C", "8D", "8E", "8F", "90", "91", "92", "93", "94", "95",
            "96", "97", "98", "99", "9A", "9B", "9C", "9D", "9E", "9F",
            "A0", "A1", "A2", "A3", "A4", "A5", "A6", "A7", "A8", "A9",
            "AA", "AB", "AC", "AD", "AE", "AF", "B0", "B1", "B2", "B3",
            "B4", "B5", "B6", "B7", "B8", "B9", "BA", "BB", "BC", "BD",
            "BE", "BF", "C0", "C1", "C2", "C3", "C4", "C5", "C6", "C7",
            "C8", "C9", "CA", "CB", "CC", "CD", "CE", "CF", "D0", "D1",
            "D2", "D3", "D4", "D5", "D6", "D7", "D8", "D9", "DA", "DB",
            "DC", "DD", "DE", "DF", "E0", "E1", "E2", "E3", "E4", "E5",
            "E6", "E7", "E8", "E9", "EA", "EB", "EC", "ED", "EE", "EF",
            "F0", "F1", "F2", "F3", "F4", "F5", "F6", "F7", "F8", "F9",
            "FA", "FB", "FC", "FD", "FE", "FF"
    };

    public static int BitsRequiredForNumber(uint n)
    {
        for (int num = 31; num >= 0; num--)
        {
            uint num2 = (uint)(1 << num);
            if ((n & num2) == num2)
            {
                return num + 1;
            }
        }

        return 0;
    }

    public static int BitsRequiredForNumber(int n)
    {
        for (int num = 31; num >= 0; num--)
        {
            int num2 = 1 << num;
            if ((n & num2) == num2)
            {
                return num + 1;
            }
        }

        return 0;
    }

    public static bool IsPowerOfTwo(uint x)
    {
        if (x != 0)
        {
            return (x & (x - 1)) == 0;
        }

        return false;
    }

    public static bool IsMultipleOf8(uint value)
    {
        if (value != 0)
        {
            return value >> 3 << 3 == value;
        }

        return false;
    }

    public static bool IsMultipleOf8(int value)
    {
        if (value > 0)
        {
            return value >> 3 << 3 == value;
        }

        return false;
    }

    public static uint NextPow2(uint v)
    {
        v--;
        v |= v >> 1;
        v |= v >> 2;
        v |= v >> 4;
        v |= v >> 8;
        v |= v >> 16;
        v++;
        return v;
    }

    public static int HighBit(uint v)
    {
        if (v == 0)
        {
            return 0;
        }

        int num = 0;
        do
        {
            num++;
        }
        while ((v >>= 1) != 0);
        return num;
    }

    public static int BytesRequired(int bits)
    {
        return bits + 7 >> 3;
    }

    public static int SeqDistance(uint from, uint to, int shift)
    {
        from <<= shift;
        to <<= shift;
        return (int)(from - to) >> shift;
    }

    public static int SeqDistance(ushort from, ushort to, int shift)
    {
        from = (ushort)(from << shift);
        to = (ushort)(to << shift);
        return (short)(from - to) >> shift;
    }

    public static uint SeqNext(uint seq, uint mask)
    {
        seq++;
        seq &= mask;
        return seq;
    }

    public static ushort SeqNext(ushort seq, ushort mask)
    {
        seq = (ushort)(seq + 1);
        seq = (ushort)(seq & mask);
        return seq;
    }

    public static ushort SeqPrev(ushort seq, ushort mask)
    {
        seq = (ushort)(seq - 1);
        seq = (ushort)(seq & mask);
        return seq;
    }

    internal static bool IsSet(uint mask, uint flag)
    {
        return (mask & flag) == flag;
    }

    internal static ushort Clamp(ushort value, ushort min, ushort max)
    {
        if (value < min)
        {
            return min;
        }

        if (value > max)
        {
            return max;
        }

        return value;
    }

    internal static float Clamp(float value, float min, float max)
    {
        if (value < min)
        {
            return min;
        }

        if (value > max)
        {
            return max;
        }

        return value;
    }

    internal static int Clamp(int value, int min, int max)
    {
        if (value < min)
        {
            return min;
        }

        if (value > max)
        {
            return max;
        }

        return value;
    }

    internal static uint Clamp(uint value, uint min, uint max)
    {
        if (value < min)
        {
            return min;
        }

        if (value > max)
        {
            return max;
        }

        return value;
    }

    internal static byte Clamp(byte value, byte min, byte max)
    {
        if (value < min)
        {
            return min;
        }

        if (value > max)
        {
            return max;
        }

        return value;
    }

    public static string ByteToHex(byte value)
    {
        return _byteHexValue[value];
    }

    public unsafe static string PrintBytesHex(byte* buffer, int length)
    {
        StringBuilder stringBuilder = new StringBuilder();
        for (int i = 0; i < length; i++)
        {
            stringBuilder.Append(_byteHexValue[buffer[i]]);
            stringBuilder.Append(" ");
        }

        return stringBuilder.ToString();
    }
}