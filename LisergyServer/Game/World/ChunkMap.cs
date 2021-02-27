using Game;
using Game.Pathfinder;
using Game.World;
using System.Collections.Generic;

namespace Game
{
    public class ChunkMap
    {
        private Chunk[,] _chunkMap;
        private CachedChunkMap _cache;
        private GameWorld _world;

        public Dictionary<ChunkFlag, List<Chunk>> ByFlags = new Dictionary<ChunkFlag, List<Chunk>>();
        public GameWorld World { get => _world; }

        public int QtdChunksX { get => _chunkMap.GetLength(0); }
        public int QtdChunksY { get => _chunkMap.GetLength(1); }
        public int QtdTilesX { get => QtdChunksX * GameWorld.CHUNK_SIZE; }
        public int QtdTilesY { get => QtdChunksY * GameWorld.CHUNK_SIZE; }

        public ChunkMap(GameWorld world)
        {
            var sizeX = world.SizeX / GameWorld.CHUNK_SIZE;
            var sizeY = world.SizeY / GameWorld.CHUNK_SIZE;
            _chunkMap = new Chunk[sizeX, sizeY];
            _cache = new CachedChunkMap(this);
            _world = world;
            Log.Debug($"Initialized chunk map {sizeX}x{sizeY}");
        }

        public bool ValidCoords(int tileX, int tileY)
        {
            return tileX >= 0 && tileX < QtdTilesX && tileY >= 0 && tileY < QtdTilesY;
        }

        public Chunk GetChunk(int chunkX, int chunkY)
        {
            return _chunkMap[chunkX, chunkY];
        }

        public List<PathFinderNode> FindPath(Tile from, Tile to)
        {
            return new PathFinder(_cache).FindPath(new Position(from.X, from.Y), new Position(to.X, to.Y));
        }

        public Chunk GetUnnocupiedNewbieChunk()
        {
            var startingChunks = ByFlags[ChunkFlag.NEWBIE_CHUNK];
            foreach (var chunk in startingChunks)
            {
                if (!chunk.Flags.HasFlag(ChunkFlag.OCCUPIED))
                    return chunk;
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

        public virtual Chunk GetTileChunk(int tileX, int tileY)
        {
            int chunkX = tileX >> GameWorld.CHUNK_SIZE_BITSHIFT;
            var chunkY = tileY >> GameWorld.CHUNK_SIZE_BITSHIFT;
            return GetChunk(chunkX, chunkY);
        }

        public virtual Tile GetTile(int tileX, int tileY)
        {
            var internalTileX = tileX % GameWorld.CHUNK_SIZE;
            var internalTileY = tileY % GameWorld.CHUNK_SIZE;
            return GetTileChunk(tileX, tileY).GetTile(internalTileX, internalTileY);
        }

        public virtual Tile GetTileFromCache(int tileX, int tileY)
        {
            return _cache.GetTile(tileX, tileY);
        }

    }
}
