using AStar;
using Game.Tile;
using System.Collections.Generic;
using System.Linq;

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
        IEnumerable<TileVector> FindPath(TileEntity from, TileEntity to);

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
    public unsafe class ServerChunkMap : IChunkMap
    {
        private Chunk[,] _chunkMap;
        private CachedChunkMap _cache;
        private IPathfinder _pathfinder;
        private Dictionary<ChunkFlag, List<Chunk>> _chunksByFlags = new Dictionary<ChunkFlag, List<Chunk>>();
        public (int x, int y) TilemapDimensions { get; private set; }
        public (int x, int y) ChunkMapDimensions { get; private set; }
        public IGameWorld World { get; private set; }

        public ServerChunkMap(GameWorld world, int tilesAmtX, int tilesAmtY)
        {
            var sizeX = tilesAmtX / GameWorld.CHUNK_SIZE;
            var sizeY = tilesAmtY / GameWorld.CHUNK_SIZE;
            _chunkMap = new Chunk[sizeX, sizeY];
            TilemapDimensions = (_chunkMap.GetLength(0) * GameWorld.CHUNK_SIZE, _chunkMap.GetLength(1) * GameWorld.CHUNK_SIZE);
            ChunkMapDimensions = (_chunkMap.GetLength(0), _chunkMap.GetLength(1));
            _cache = new CachedChunkMap(this);
            _pathfinder = new AStarSearch(_cache);
            World = world;
        }

        public bool ValidCoords(in int tileX, in int tileY)
        {
            var dim = TilemapDimensions;
            return tileX >= 0 && tileX < dim.Item1 && tileY >= 0 && tileY < dim.Item2;
        }

       
        public Chunk GetChunk(in int chunkX, in int chunkY)
        {
            var chunk = _chunkMap[chunkX, chunkY];
            if(chunk == null)
            {
                chunk = new Chunk(this, chunkX, chunkY);
                _chunkMap[chunk.X, chunk.Y] = chunk;
            }
            return chunk;
        }

       
        public IEnumerable<TileVector> FindPath(TileEntity from, TileEntity to)
        {
            return _pathfinder.Find(from.Position, to.Position);
        }

       
        public Chunk GetUnnocupiedNewbieChunk()
        {
            var startingChunks = _chunksByFlags[ChunkFlag.NEWBIE_CHUNK];
            foreach (var position in startingChunks)
            {
                var chunk = GetChunk(position.X, position.Y);
                if (!chunk.Flags.HasFlag(ChunkFlag.OCCUPIED))
                    return chunk;
            }
            return null;
        }

       
        public void SetFlag(int chunkX, int chunkY, ChunkFlag flag)
        {
            var chunk = GetChunk(chunkX, chunkY);
            chunk.SetFlag((byte)flag);
            if (!_chunksByFlags.ContainsKey(flag))
                _chunksByFlags.Add(flag, new List<Chunk>());
            _chunksByFlags[flag].Add(chunk);
        }

       
        public IEnumerable<Chunk> AllChunks()
        {
            for (var x = 0; x < _chunkMap.GetLength(0); x++)
                for (var y = 0; y < _chunkMap.GetLength(1); y++)
                    yield return GetChunk(x, y);
        }

       
        public virtual Chunk GetTileChunk(in int tileX, in int tileY)
        {
            int chunkX = tileX >> GameWorld.CHUNK_SIZE_BITSHIFT;
            var chunkY = tileY >> GameWorld.CHUNK_SIZE_BITSHIFT;
            return GetChunk(chunkX, chunkY);
        }

       
        public virtual TileEntity GetTile(in int tileX, in int tileY)
        {
            if (!ValidCoords(tileX, tileY)) return null;
            var internalTileX = tileX % GameWorld.CHUNK_SIZE;
            var internalTileY = tileY % GameWorld.CHUNK_SIZE;
            return GetTileChunk(tileX, tileY).GetTile(internalTileX, internalTileY);
        }

       
        public virtual void CreateMap(in ushort sizeX, in ushort sizeY)
        {

        }
    }
}
