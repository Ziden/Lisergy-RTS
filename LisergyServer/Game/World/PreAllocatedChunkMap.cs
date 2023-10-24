using Game.Pathfinder;
using Game.Systems.FogOfWar;
using Game.Systems.Tile;
using Game.Tile;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Game.World
{
    /// <summary>
    /// Represents a map that is divided by chunks.
    /// </summary>
    public interface IChunkMap
    {
        /// <summary>
        /// Gets the world this chunk map belongs to
        /// </summary>
        IGameWorld World { get; }

        /// <summary>
        /// Checks if a given tile coordinates is valid 
        /// </summary>
        bool ValidCoords(in int tileX, in int tileY);

        /// <summary>
        /// Gets a given chunk by its X Y coords
        /// </summary>
        Chunk GetChunk(in int chunkX, in int chunkY);

        /// <summary>
        /// Finds a path between source and destination.
        /// Can return an empty list if no path is found.
        /// </summary>
        List<PathFinderNode> FindPath(TileEntity from, TileEntity to);

        /// <summary>
        /// Creates the chunk map instance and allocate needed memory
        /// </summary>
        void CreateMap(in ushort sizeX, in ushort sizeY);

        /// <summary>
        /// Gets a given tile instance
        /// </summary>
        TileEntity GetTile(in int tileX, in int tileY);

        /// <summary>
        /// Gets the dimensions of the map in amount of tiles
        /// </summary>
        public (int x, int y) TilemapDimensions { get; }

        /// <summary>
        /// Gets the dimensions of the map in amount of chunks
        /// </summary>
        public (int x, int y) ChunkMapDimensions { get; }
    }

    /// <summary>
    /// Preallocate all chunks for faster acessing
    /// </summary>
    public unsafe class PreAllocatedChunkMap : IChunkMap
    {
        private Chunk[,] _chunkMap;
        private CachedChunkMap _cache;
        private Dictionary<ChunkFlag, List<Chunk>> _chunksByFlags = new Dictionary<ChunkFlag, List<Chunk>>();
        public (int x, int y) TilemapDimensions { get; private set; }
        public (int x, int y) ChunkMapDimensions { get; private set; }
        public IGameWorld World { get; private set; }

        public PreAllocatedChunkMap(GameWorld world, int tilesAmtX, int tilesAmtY)
        {
            var sizeX = tilesAmtX / GameWorld.CHUNK_SIZE;
            var sizeY = tilesAmtY / GameWorld.CHUNK_SIZE;
            _chunkMap = new Chunk[sizeX, sizeY];
            TilemapDimensions = (_chunkMap.GetLength(0) * GameWorld.CHUNK_SIZE, _chunkMap.GetLength(1) * GameWorld.CHUNK_SIZE);
            ChunkMapDimensions = (_chunkMap.GetLength(0), _chunkMap.GetLength(1));
            _cache = new CachedChunkMap(this);
            World = world;
        }

       
        public bool ValidCoords(in int tileX, in int tileY)
        {
            var dim = TilemapDimensions;
            return tileX >= 0 && tileX < dim.Item1 && tileY >= 0 && tileY < dim.Item2;
        }

       
        public Chunk GetChunk(in int chunkX, in int chunkY)
        {
            return _chunkMap[chunkX, chunkY];
        }

       
        public List<PathFinderNode> FindPath(TileEntity from, TileEntity to)
        {
            return new PathFinder(_cache).FindPath(new Position(from.X, from.Y), new Position(to.X, to.Y));
        }

       
        public Chunk GetUnnocupiedNewbieChunk()
        {
            var startingChunks = _chunksByFlags[ChunkFlag.NEWBIE_CHUNK];
            foreach (var position in startingChunks)
            {
                var chunk = _chunkMap[position.X, position.Y];
                if (!chunk.Flags.HasFlag(ChunkFlag.OCCUPIED))
                    return _chunkMap[position.X, position.Y];
            }
            return null;
        }

       
        public void SetFlag(int chunkX, int chunkY, ChunkFlag flag)
        {
            var chunk = _chunkMap[chunkX, chunkY];
            chunk.SetFlag((byte)flag);
            if (!_chunksByFlags.ContainsKey(flag))
                _chunksByFlags.Add(flag, new List<Chunk>());
            _chunksByFlags[flag].Add(chunk);
        }

       
        public IEnumerable<Chunk> AllChunks()
        {
            for (var x = 0; x < _chunkMap.GetLength(0); x++)
                for (var y = 0; y < _chunkMap.GetLength(1); y++)
                    yield return _chunkMap[x, y];
        }

       
        public virtual Chunk GetTileChunk(in int tileX, in int tileY)
        {
            int chunkX = tileX >> GameWorld.CHUNK_SIZE_BITSHIFT;
            var chunkY = tileY >> GameWorld.CHUNK_SIZE_BITSHIFT;
            return GetChunk(chunkX, chunkY);
        }

       
        public virtual TileEntity GetTile(in int tileX, in int tileY)
        {
            var internalTileX = tileX % GameWorld.CHUNK_SIZE;
            var internalTileY = tileY % GameWorld.CHUNK_SIZE;
            return GetTileChunk(tileX, tileY).GetTile(internalTileX, internalTileY);
        }

       
        public virtual void CreateMap(in ushort sizeX, in ushort sizeY)
        {
            var maxChunkX = sizeX >> GameWorld.CHUNK_SIZE_BITSHIFT;
            var maxChunkY = sizeY >> GameWorld.CHUNK_SIZE_BITSHIFT;
            for (ushort chunkX = 0; chunkX < maxChunkX; chunkX++)
            {
                for (ushort chunkY = 0; chunkY < maxChunkY; chunkY++)
                {
                    var chunk = new Chunk(this, chunkX, chunkY);
                    for (ushort x = 0; x < GameWorld.CHUNK_SIZE; x++)
                        for (ushort y = 0; y < GameWorld.CHUNK_SIZE; y++)
                            chunk.CreateTile(x, y);
                    _chunkMap[chunk.X, chunk.Y] = chunk;
                }
            }
        }
    }
}
