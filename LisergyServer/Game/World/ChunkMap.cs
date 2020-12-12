using Game;
using System.Collections.Generic;

namespace Game
{
    public class ChunkMap
    {
        private Chunk[,] _chunkMap;

        public Dictionary<ChunkFlag, List<Chunk>> ByFlags = new Dictionary<ChunkFlag, List<Chunk>>();

        public ChunkMap(int sizeX, int sizeY)
        {
            _chunkMap = new Chunk[sizeX, sizeY];
            Log.Debug($"Initialized chunk map {sizeX}x{sizeY}");
        }

        public Chunk GetChunk(int chunkX, int chunkY)
        {
            return _chunkMap[chunkX, chunkY];
        }

        public Chunk GetUnnocupiedNewbieChunk()
        {
            var startingChunks = ByFlags[ChunkFlag.NEWBIE_CHUNK];
            foreach (var chunk in startingChunks)
            {
                if (!chunk.HasFlag(ChunkFlag.OCCUPIED))
                {
                    return chunk;
                }
            }
            return null;
        }

        public void Add(Chunk c)
        {
            Log.Debug($"Adding chunk {c.X} {c.Y}");
            _chunkMap[c.X, c.Y] = c;
        }

        public void SetFlag(int chunkX, int chunkY, ChunkFlag flag)
        {
            var chunk = this._chunkMap[chunkX, chunkY];
            chunk.Flags = chunk.Flags |= (byte)flag;
            if (!ByFlags.ContainsKey(flag))
                ByFlags.Add(flag, new List<Chunk>());
            ByFlags[flag].Add(chunk);
        }

        public IEnumerable<Chunk> AllChunks()
        {
            for (var x = 0; x < _chunkMap.GetLength(0); x++)
                for (var y = 0; y < _chunkMap.GetLength(1); y++)
                    yield return _chunkMap[x, y];
        }

     
    }
}
