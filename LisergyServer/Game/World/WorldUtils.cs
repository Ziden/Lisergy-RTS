using Game.Engine.DataTypes;
using Game.Tile;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.World
{
    public static class WorldUtils
    {
        internal static DeterministicRandom _random = new DeterministicRandom();

        public static T RandomElement<T>(this IEnumerable<T> input)
        {
            return input.ElementAt(_random.Next(input.Count()));
        }

        public static DeterministicRandom Random { get => _random; }

        public static void SetRandomSeed(int seed)
        {
            _random = new DeterministicRandom(seed);
        }

        // Gets the amount of bits required to allocate a given number
        // This will be used to get the amount 
        public static int BitsRequired(this int num)
        {
            const int mask = int.MinValue;
            int leadingZeros = 0;
            for (; leadingZeros < 32; leadingZeros++)
            {
                if ((num & mask) != 0)
                    break;
                num <<= 1;
            }
            return 32 - leadingZeros;
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

        public static Direction GetDirection(this TileEntity tile, TileEntity otherTile)
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

        private static ushort ONE = 1;

        public static TileEntity FindTileWithId(this Chunk chunk, byte tileID)
        {
            var tiles = chunk.Tiles;
            var tries = 10;
            while (tries > 0)
            {
                var rndX = _random.Next(0, tiles.GetLength(0));
                var rndY = _random.Next(0, tiles.GetLength(1));
                TileEntity tile = chunk.GetTile(rndX, rndY);
                if (tile.SpecId == tileID)
                    return tile;
                tries--;
            }
            return null;
        }

        public static TileEntity GetNeighbor(this TileEntity tile, Direction d)
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

        public static int Distance(this TileEntity tile, TileEntity t2)
        {
            return Math.Abs(tile.X - t2.X) + Math.Abs(tile.Y - t2.Y);
        }

        public static int Distance(this Location tile, in Location t2)
        {
            return Math.Abs(tile.X - t2.X) + Math.Abs(tile.Y - t2.Y);
        }

        public static TileEntity GetNeighborInRange(this TileEntity source, int range, Direction d)
        {
            while (range > 0)
            {
                source = source.GetNeighbor(d);
                range--;
            }
            return source;
        }


        public static IEnumerable<TileEntity> GetAOE(this TileEntity tile, ushort radius)
        {
            for (var xx = -radius; xx <= radius; xx++)
                for (var yy = -radius; yy <= radius; yy++)
                {
                    if (xx == -radius && yy == -radius) continue;
                    if (xx == -radius && yy == radius) continue;
                    if (xx == radius && yy == -radius) continue;
                    if (xx == radius && yy == radius) continue;
                    var x = tile.X + xx;
                    var y = tile.Y + yy;
                    if (tile.Chunk.Map.ValidCoords(x, y))
                        yield return tile.Chunk.Map.GetTile(x, y);
                }
        }
    }
}
