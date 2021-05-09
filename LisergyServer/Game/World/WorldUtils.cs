using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.World
{
    public static class EnumerableHelper<E>
    {
        static EnumerableHelper()
        {
            WorldUtils._random = new Random();
        }

        public static T RandomElement<T>(IEnumerable<T> input)
        {
            return input.ElementAt(WorldUtils._random.Next(input.Count()));
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
        internal static Random _random = new Random();

        public static Random Random { get => _random; }

        public static void SetRandomSeed(int seed)
        {
            _random = new Random(seed);
        }

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

        public static Direction GetDirection(this Tile tile, Tile otherTile)
        {
            if (tile.X == otherTile.X - 1 && tile.Y == otherTile.Y)
                return Direction.EAST;
            else if (tile.X == otherTile.X + 1 && tile.Y == otherTile.Y)
                return Direction.WEST;
            else if (tile.X == otherTile.X && tile.Y == otherTile.Y - 1)
                return Direction.NORTH;
              else if (tile.X == otherTile.X && tile.Y == otherTile.Y + 1)
                return Direction.SOUTH;
            return Direction.NONE;
        }

        public static Tile FindTileWithId(this Chunk chunk, byte tileID)
        {
            var tiles = chunk.Tiles;
            var tries = 10;
            while (tries > 0)
            {
                var rndX = WorldUtils._random.Next(0, tiles.GetLength(0));
                var rndY = WorldUtils._random.Next(0, tiles.GetLength(1));
                Tile tile = tiles[rndX, rndY];
                if (tile.TileId == tileID)
                    return tile;
                tries--;
            }
            return null;
        }

        public static Tile GetNeighbor(this Tile tile, Direction d)
        {
            switch (d)
            {
                case Direction.EAST: return tile.Chunk.Map.GetTile(tile.X + 1, tile.Y);
                case Direction.WEST: return tile.Chunk.Map.GetTile(tile.X - 1, tile.Y);
                case Direction.SOUTH: return tile.Chunk.Map.GetTile(tile.X, tile.Y - 1);
                case Direction.NORTH: return tile.Chunk.Map.GetTile(tile.X, tile.Y + 1);
            }
            return null;
        }

        public static IEnumerable<Tile> GetAOE(this Tile tile, ushort radius)
        {
            for (var xx = tile.X - radius; xx <= tile.X + radius; xx++)
                for (var yy = tile.Y - radius; yy <= tile.Y + radius; yy++)
                    if (tile.Chunk.Map.ValidCoords(xx, yy))
                        yield return tile.Chunk.Map.GetTile(xx, yy);
        }
    }
}
