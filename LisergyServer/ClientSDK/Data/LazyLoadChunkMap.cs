using Game.Pathfinder;
using Game.Tile;
using Game;
using Game.World;
using System.Collections.Generic;
using Game.DataTypes;

namespace ClientSDK.Data
{
    /// <summary>
    /// Chunk map that can lazy load tiles and chunk into memory
    /// </summary>
    public class LazyLoadChunkMap : IChunkMap
    {
        private Dictionary<GameId, Chunk> _chunks = new Dictionary<GameId, Chunk>();
        
        private CachedChunkMap _cache;

        public (int x, int y) TilemapDimensions { get; private set; }

        public LazyLoadChunkMap(IGameWorld world, int tilesAmtX, int tilesAmtY) : base()
        {
            var sizeX = tilesAmtX / GameWorld.CHUNK_SIZE;
            var sizeY = tilesAmtY / GameWorld.CHUNK_SIZE;
            TilemapDimensions = (tilesAmtX, tilesAmtY);
            _cache = new CachedChunkMap(this);
            World = world;
            Log.Debug($"Initialized lazy loaded chunk map {sizeX}x{sizeY}");
        }

        public IGameWorld World { get; private set; }

        public bool ValidCoords(in int tileX, in int tileY)
        {
            return tileX >= 0 && tileX < TilemapDimensions.x && tileY >= 0 && tileY < TilemapDimensions.y;
        }

        public List<PathFinderNode> FindPath(TileEntity from, TileEntity to)
        {
            return new PathFinder(_cache).FindPath(new Position(from.X, from.Y), new Position(to.X, to.Y));
        }

        public Chunk GetChunk(in int chunkX, in int chunkY)
        {
            var chunkId = new GameId(new Position(chunkX, chunkY));
            if (!_chunks.TryGetValue(chunkId, out var chunk))
            {
                chunk = new Chunk(this, (ushort)chunkX, (ushort)chunkY);
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

        public virtual TileEntity GetTile(in int tileX, in int tileY)
        {
            if (!ValidCoords(tileX, tileY)) return null!;
            var internalTileX = tileX % GameWorld.CHUNK_SIZE;
            var internalTileY = tileY % GameWorld.CHUNK_SIZE;
            var chunk = GetTileChunk(tileX, tileY);
            var tile = chunk.GetTile(internalTileX, internalTileY);
            if(tile == null)
            {
                tile = chunk.CreateTile(internalTileX, internalTileY);
            }
            return tile;
        }

        public virtual void CreateMap(in ushort sizeX, in ushort sizeY) { }

        public virtual TileEntity GenerateTile(ref Chunk c, in int tileX, in int tileY)
        {
            var tile = c.CreateTile(tileX, tileY);
            return tile;
        }
    }
}
