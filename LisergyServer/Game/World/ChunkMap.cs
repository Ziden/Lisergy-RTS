using Game;
using Game.Pathfinder;
using Game.World;
using Game.World.Components;
using Game.World.Data;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Game
{
    public unsafe class ChunkMap
    {
        private Chunk[,] _chunkMap;
        private CachedChunkMap _cache;

        public Dictionary<ChunkFlag, List<Position>> ByFlags = new Dictionary<ChunkFlag, List<Position>>();

        public int QtdChunksX { get => _chunkMap.GetLength(0); }
        public int QtdChunksY { get => _chunkMap.GetLength(1); }
        public int QtdTilesX { get => QtdChunksX * GameWorld.CHUNK_SIZE; }
        public int QtdTilesY { get => QtdChunksY * GameWorld.CHUNK_SIZE; }

        public GameWorld World { get; private set; }

        public ChunkMap(GameWorld world, int tilesAmtX, int tilesAmtY)
        {
            var sizeX = tilesAmtX / GameWorld.CHUNK_SIZE;
            var sizeY = tilesAmtY / GameWorld.CHUNK_SIZE;
            _chunkMap = new Chunk[sizeX, sizeY];
            _cache = new CachedChunkMap(this);
            this.World = world;
            Log.Debug($"Initialized chunk map {sizeX}x{sizeY}");
        }

        public bool ValidCoords(int tileX, int tileY)
        {
            return tileX >= 0 && tileX < QtdTilesX && tileY >= 0 && tileY < QtdTilesY;
        }

        public ref Chunk GetChunk(int chunkX, int chunkY)
        {
            return ref _chunkMap[chunkX, chunkY];
        }

        public ref Chunk GetChunk(in Position pos)
        {
            return ref _chunkMap[pos.X, pos.Y];
        }

        public List<PathFinderNode> FindPath(Tile from, Tile to)
        {
            return new PathFinder(_cache).FindPath(new Position(from.X, from.Y), new Position(to.X, to.Y));
        }

        public Chunk GetUnnocupiedNewbieChunk()
        {
            var startingChunks = ByFlags[ChunkFlag.NEWBIE_CHUNK];
            foreach (var position in startingChunks)
            {
                var chunk = _chunkMap[position.X, position.Y];
                if (!chunk.Flags.HasFlag(ChunkFlag.OCCUPIED))
                    return _chunkMap[position.X, position.Y];
            }
            return null;
        }

        public void Add(ref Chunk c)
        {
            _chunkMap[c.X, c.Y] = c;
        }

        public void SetFlag(int chunkX, int chunkY, ChunkFlag flag)
        {
            var chunk = this._chunkMap[chunkX, chunkY];
            chunk.SetFlag((byte)flag);
            if (!ByFlags.ContainsKey(flag))
                ByFlags.Add(flag, new List<Position>());
            ByFlags[flag].Add(chunk.Position);
        }

        public IEnumerable<Chunk> AllChunks()
        {
            var i = 0;
            for (var x = 0; x < _chunkMap.GetLength(0); x++)
            {
                for (var y = 0; y < _chunkMap.GetLength(1); y++)
                {
                    yield return _chunkMap[x, y];
                }
            }      
        }

        public virtual ref Chunk GetTileChunk(int tileX, int tileY)
        {
            int chunkX = tileX >> GameWorld.CHUNK_SIZE_BITSHIFT;
            var chunkY = tileY >> GameWorld.CHUNK_SIZE_BITSHIFT;
            return ref GetChunk(chunkX, chunkY);
        }

        public virtual Tile GetTile(int tileX, int tileY)
        {
            var internalTileX = tileX % GameWorld.CHUNK_SIZE;
            var internalTileY = tileY % GameWorld.CHUNK_SIZE;
            return GetTileChunk(tileX, tileY).GetTile(internalTileX, internalTileY);
        }

        public virtual void GenerateTiles(int sizeX, int sizeY)
        {
            var maxChunkX = sizeX >> GameWorld.CHUNK_SIZE_BITSHIFT;
            var maxChunkY = sizeY >> GameWorld.CHUNK_SIZE_BITSHIFT;
            for (var chunkX = 0; chunkX < maxChunkX; chunkX++)
            {
                for (var chunkY = 0; chunkY < maxChunkY; chunkY++)
                {
                    var tiles = new Tile[GameWorld.CHUNK_SIZE, GameWorld.CHUNK_SIZE];
                    var chunk = new Chunk(this, chunkX, chunkY, tiles);
                    for (var x = 0; x < GameWorld.CHUNK_SIZE; x++)
                    {
                        for (var y = 0; y < GameWorld.CHUNK_SIZE; y++)
                        {
                            var tileX = chunkX * GameWorld.CHUNK_SIZE + x;
                            var tileY = chunkY * GameWorld.CHUNK_SIZE + y;
                            tiles[x, y] = GenerateTile(ref chunk, tileX, tileY);
                        }
                    }
                    this.Add(ref chunk);
                }
            }
        }

        public virtual void ClearTile(Tile t)
        {
            t.Components.Get<TileVisibility>().EntitiesViewing.Clear();
            t.Components.Get<TileVisibility>().PlayersViewing.Clear();
            t.Components.Get<TileHabitants>().EntitiesIn.Clear();
            t.Components.Get<TileHabitants>().StaticEntity = null;
        }


        public virtual Tile GenerateTile(ref Chunk c, int tileX, int tileY)
        {
            var tile = c.CreateTile(tileX, tileY);
            tile.Components.Add(new TileVisibility());
            tile.Components.Add(new TileHabitants());
            return tile;
        }
    }
}
