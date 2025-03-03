using Game.Engine.DataTypes;
using Game.Engine.Pathfinder;
using Game.Tile;
using Game.World;
using System.Collections.Generic;

namespace ClientSDK.Data
{
    /// <summary>
    /// Chunk map that can lazy load tiles and chunk into memory
    /// </summary>
    public class LazyLoadChunkMap : IChunkMap
    {
        private Dictionary<GameId, Chunk> _chunks = new Dictionary<GameId, Chunk>();

        private PathfindingChunkMap _cache;

        public (int x, int y) ChunkMapDimensions { get; private set; }

        public (int x, int y) TilemapDimensions { get; private set; }

        public LazyLoadChunkMap(IGameWorld world, int tilesAmtX, int tilesAmtY) : base()
        {
            var sizeX = tilesAmtX / GameWorld.CHUNK_SIZE;
            var sizeY = tilesAmtY / GameWorld.CHUNK_SIZE;
            TilemapDimensions = (tilesAmtX, tilesAmtY);
            ChunkMapDimensions = (sizeX, sizeY);
            _cache = new PathfindingChunkMap(this);
            World = world;
        }

        public IGameWorld World { get; private set; }

        public bool ValidCoords(in int tileX, in int tileY)
        {
            return tileX >= 0 && tileX < TilemapDimensions.x && tileY >= 0 && tileY < TilemapDimensions.y;
        }

        public IEnumerable<Location> FindPath(TileModel from, TileModel to)
        {
            return new AStarSearch(_cache).Find(from.Position, to.Position);
        }

        public Chunk GetChunk(in int chunkX, in int chunkY)
        {
            var chunkId = new GameId(new Location(chunkX, chunkY));
            if (!_chunks.TryGetValue(chunkId, out var chunk))
            {
                chunk = new Chunk(World, (ushort)chunkX, (ushort)chunkY);
                _chunks[chunkId] = chunk;
            }
            return chunk;
        }

        public Chunk GetTileChunk(in int tileX, in int tileY)
        {
            var chunkX = tileX >> GameWorld.CHUNK_SIZE_BITSHIFT;
            var chunkY = tileY >> GameWorld.CHUNK_SIZE_BITSHIFT;
            var chunk = GetChunk(chunkX, chunkY);
            return chunk;
        }

        public virtual TileModel GetTile(in int tileX, in int tileY)
        {
            if (!ValidCoords(tileX, tileY)) return null!;
            var internalTileX = tileX % GameWorld.CHUNK_SIZE;
            var internalTileY = tileY % GameWorld.CHUNK_SIZE;
            var chunk = GetTileChunk(tileX, tileY);
            var tile = chunk.GetTile(internalTileX, internalTileY);
            if (tile == null)
            {
                tile = chunk.CreateTile(internalTileX, internalTileY);
                tile.Logic.DeltaCompression.Clear();
            }
            return tile;
        }

        public virtual void CreateMap(in ushort sizeX, in ushort sizeY) { }

        public virtual TileModel GenerateTile(ref Chunk c, in int tileX, in int tileY)
        {
            var tile = c.CreateTile(tileX, tileY);
            return tile;
        }

        public TileModel GetTile(in Location p)
        {
            return GetTile(p.X, p.Y);
        }

        public Chunk GetTileChunk(in Location p)
        {
            return GetTileChunk(p.X, p.Y);
        }
    }
}
