using Game.Tile;

namespace Game.Systems.World
{
    /// <summary>
    /// Used for pathfinding.
    /// </summary>
    public class CachedChunkMap
    {
        public TileEntity[,] array;
        private ChunkMap _chunkMap;

        public CachedChunkMap(ChunkMap chunkMap)
        {
            _chunkMap = chunkMap;
            array = new TileEntity[SizeX, SizeY];
        }

        public TileEntity GetTile(int x, int y)
        {
            var cached = array[x, y];
            if (cached == null)
            {
                cached = _chunkMap.GetTile(x, y);
                array[x, y] = cached;
            }
            return cached;
        }

        public TileEntity this[int x, int y]
        {
            get => GetTile(x, y);
        }

        public int SizeX { get => _chunkMap.QtdChunksX * GameWorld.CHUNK_SIZE; }
        public int SizeY { get => _chunkMap.QtdChunksY * GameWorld.CHUNK_SIZE; }
    }
}
