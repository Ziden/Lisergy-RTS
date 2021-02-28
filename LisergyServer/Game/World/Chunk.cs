using System;
using System.Collections.Generic;

namespace Game
{
    [Flags]
    public enum ChunkFlag : byte
    {
        NEWBIE_CHUNK = 0b00000001,
        OCCUPIED = 0b00000010
    }

    [Serializable]
    public class Chunk
    {
        private byte _flags;
        private Tile[,] _tiles;

        public virtual ushort X { get; private set; }
        public virtual ushort Y { get; private set; }
        public virtual ChunkMap ChunkMap { get; private set; }
        public virtual byte Flags { get => _flags; set => _flags = value; }
        public virtual Tile[,] Tiles { get => _tiles; private set => _tiles = value; }
        public Chunk(ChunkMap w, int x, int y, Tile[,] tiles)
        {
            this.X = (ushort)x;
            this.Y = (ushort)y;
            Tiles = tiles;
            this.ChunkMap = w;
        }

        public Tile GetTile(int x, int y)
        {
            return Tiles[x, y];
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
