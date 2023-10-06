using Game.Tile;

namespace Game.World
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

        public TileEntity GetTile(in int x, in int y)
        {
            var cached = array[x, y];
            if (cached == null)
            {
                cached = _chunkMap.GetTile(x, y);
                array[x, y] = cached;
            }
            return cached;
        }

        public TileEntity this[in int x, in int y]
        {
            get => GetTile(x, y);
        }

        public int SizeX { get => _chunkMap.QtdChunksX * GameWorld.CHUNK_SIZE; }
        public int SizeY { get => _chunkMap.QtdChunksY * GameWorld.CHUNK_SIZE; }
    }
}
