using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.World
{

    public static class EnumerableHelper<E>
    {
        private static Random r;

        static EnumerableHelper()
        {
            r = new Random();
        }

        public static T RandomElement<T>(IEnumerable<T> input)
        {
            return input.ElementAt(r.Next(input.Count()));
        }

    }

    public static class EnumerableExtensions
    {
        public static T RandomElement<T>(this IEnumerable<T> input)
        {
            return EnumerableHelper<T>.RandomElement(input);
        }
    }

    public static class WorldUtils
    {
        public static void RemoveString(this string[] array, string obj)
        {
            for (var i = 0; i < array.Length; i++)
            {
                if (array[i] == obj)
                {
                    array[i] = null;
                    return;
                }
            }
        }

        public static int FilledSlots(this string[] array)
        {
            var amt = 0;
            for (var i = 0; i < array.Length; i++)
            {
                if (array[i] != null)
                {
                    amt++;
                }
            }
            return amt;
        }

        public static void AddString(this string [] array, string obj)
        {
            for(var i  = 0; i < array.Length; i ++)
            {
                if(array[i]==null)
                {
                    array[i] = obj;
                    return;
                }
            }
        }

        // Gets the amount of bits required to allocate a given number
        // This will be used to get the amount 
        public static int BitsRequired(this int num)
        {
            const int mask = Int32.MinValue;
            int leadingZeros = 0;
            for (; leadingZeros < 32; leadingZeros++)
            {
                if ((num & mask) != 0)
                    break;
                num <<= 1;
            }
            return 32 - leadingZeros;
        }

        public static int ToChunkCoordinate(this int num)
        {
            return num >> GameWorld.CHUNK_SIZE_BITSHIFT;
        }

        public static int ToTileCoordinate(this int num)
        {
            return num << GameWorld.CHUNK_SIZE_BITSHIFT;
        }

        public static void AddFlag<T>(this ref byte value, T flag) 
        {
            value.AddFlag((byte)(object)flag);
        }

        public static void RemoveFlag<T>(this ref byte value, T flag)
        {
            value.RemoveFlag((byte)(object)flag);
        }

        public static bool HasFlag<T>(this byte value, T flag)
        {
            return value.HasFlag((byte)(object)flag);
        }

        public static void AddFlag(this ref byte value, byte flag) 
        {
            value = value |= flag;
        }

        public static void RemoveFlag(this ref byte value, byte flag)
        {
            value = value &= (byte)~flag;
        }

        public static bool HasFlag(this byte value, byte flag)
        {
            return (value & flag) != 0;
        }

      
    }
}
