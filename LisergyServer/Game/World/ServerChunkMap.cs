using Game.Engine.Pathfinder;
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
        IEnumerable<Location> FindPath(TileModel from, TileModel to);

        /// <summary>
        /// Creates the chunk map instance and allocate needed memory
        /// </summary>
        void CreateMap(in ushort sizeX, in ushort sizeY);

        /// <summary>
        /// Gets a given tile instance
        /// </summary>
        TileModel GetTile(in int tileX, in int tileY);

        /// <summary>
        /// Gets a given tile instance
        /// </summary>
        TileModel GetTile(in Location p);

        /// <summary>
        /// Gets the chunk of a given tile location
        /// </summary>
        Chunk GetTileChunk(in Location p);
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
    // TODO: Remove chunks
    public unsafe class ServerChunkMap : IChunkMap
    {
        private Chunk[,] _chunkMap;
        private PathfindingChunkMap _cache;
        private IPathfinder _pathfinder;
        private List<Chunk> _unocuppied = new List<Chunk>();

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
            _cache = new PathfindingChunkMap(this);
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
            if (chunk == null)
            {
                chunk = new Chunk(World, chunkX, chunkY);
                _chunkMap[chunk.X, chunk.Y] = chunk;
            }
            return chunk;
        }

        public IEnumerable<Location> FindPath(TileModel from, TileModel to)
        {
            return _pathfinder.Find(from.Position, to.Position);
        }

        public Chunk GetUnnocupiedNewbieChunk()
        {
            return _unocuppied.First();
        }

        public void SetOccupied(int chunkX, int chunkY, bool oc)
        {
            var chunk = GetChunk(chunkX, chunkY);
            if (oc) _unocuppied.Remove(chunk);
            else _unocuppied.Add(chunk);
        }


        public IEnumerable<Chunk> AllChunks()
        {
            for (var x = 0; x < _chunkMap.GetLength(0); x++)
                for (var y = 0; y < _chunkMap.GetLength(1); y++)
                    yield return GetChunk(x, y);
        }

        public IEnumerable<TileModel> AllTiles()
        {
            foreach (var chunk in AllChunks())
                foreach (var tile in chunk.AllTiles())
                    yield return tile;
        }

        public virtual Chunk GetTileChunk(in int tileX, in int tileY)
        {
            int chunkX = tileX >> GameWorld.CHUNK_SIZE_BITSHIFT;
            var chunkY = tileY >> GameWorld.CHUNK_SIZE_BITSHIFT;
            return GetChunk(chunkX, chunkY);
        }

        public virtual TileModel GetTile(in int tileX, in int tileY)
        {
            if (!ValidCoords(tileX, tileY)) return null;
            var internalTileX = tileX % GameWorld.CHUNK_SIZE;
            var internalTileY = tileY % GameWorld.CHUNK_SIZE;
            return GetTileChunk(tileX, tileY).GetTile(internalTileX, internalTileY);
        }

        public virtual TileModel GetTile(in Location p) => GetTile(p.X, p.Y);
        public virtual Chunk GetTileChunk(in Location l) => GetTileChunk(l.X, l.Y);

        public virtual void CreateMap(in ushort sizeX, in ushort sizeY)
        {

        }
    }
}
