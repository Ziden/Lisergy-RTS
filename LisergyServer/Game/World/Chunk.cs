using System;
using System.Collections.Generic;

namespace Game
{
    public enum ChunkFlag
    {
        NEWBIE_CHUNK = 0b00000001,
        OCCUPIED = 0b00000010
    }

    [Serializable]
    public class Chunk
    {
        private byte _flags;
        private Tile[,] _tiles;

        public ushort X { get; private set; }
        public ushort Y { get; private set; }
        public GameWorld World { get; private set; }
        public byte Flags { get => _flags; set => _flags = value; }
        public Tile[,] Tiles { get => _tiles; private set => _tiles = value; }

        public Chunk(GameWorld w, int x, int y, Tile[,] tiles)
        {
            this.X = (ushort)x;
            this.Y = (ushort)y;
            Tiles = tiles;
            this.World = w;
        }

        public Tile GetTile(int x, int y)
        {
            return Tiles[x, y];
        }

        public bool HasFlag(ChunkFlag flag)
        {
            return (Flags & (byte)flag) == 1;
        }

        public override String ToString()
        {
            return $"<Chunk x={X} y={Y}>";
        }


        public IEnumerable<Tile> AllTiles()
        {
            for (var x = 0; x < _tiles.GetLength(0); x++)
                for (var y = 0; y < _tiles.GetLength(1); y++)
                    yield return _tiles[x, y];
        }
    }
}
