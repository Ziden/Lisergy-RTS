using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Game.World
{
    // Chunkmap cache to be used in pathfinding etc
    public class CachedChunkMap
    {
        public Tile[,] array;
        private ChunkMap _chunkMap;

        public CachedChunkMap(ChunkMap chunkMap)
        {
            this._chunkMap = chunkMap;
            array = new Tile[SizeX, SizeY];
        }

        public Tile GetTile(int x, int y)
        {
            var cached = array[x, y];
            if(cached == null)
            {
                cached = _chunkMap.GetTile(x, y);
                array[x, y] = cached;
            }
            return cached;
        }

        public Tile this[int x, int y]
        {
            get => GetTile(x, y);
        }

        public int SizeX { get => _chunkMap.QtdChunksX * GameWorld.CHUNK_SIZE; }
        public int SizeY { get => _chunkMap.QtdChunksY * GameWorld.CHUNK_SIZE; }
    }
}
