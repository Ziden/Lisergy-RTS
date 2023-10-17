using Game.DataTypes;
using Game.Systems.Player;
using Game.Systems.Tile;
using Game.Tile;
using System;
using System.Collections.Generic;

namespace Game.World
{
    public interface IGameWorld : IDisposable {

        /// <summary>
        /// Gets the game this world belongs to
        /// </summary>
        public IGame Game { get; }

        /// <summary>
        /// Gets the players that reside in this world
        /// </summary>
        public IGamePlayers Players { get; }

        /// <summary>
        /// Gets the world map
        /// </summary>
        public IChunkMap Map { get; }
    }

    public class GameWorld : IGameWorld
    {
        private GameId _id;
        public const int CHUNK_SIZE = 8;
        public static readonly int CHUNK_SIZE_BITSHIFT = CHUNK_SIZE.BitsRequired() - 1;
        public const int TILES_IN_CHUNK = CHUNK_SIZE * CHUNK_SIZE;
        public const int PLAYERS_CHUNKS = 2;

        public virtual IGame Game { get; set; }
        protected PreAllocatedChunkMap _preallocatedMap { get; set; }
        public ushort Seed { get; set; }
        public ushort SizeX { get; private set; }
        public ushort SizeY { get; private set; }
        public IGamePlayers Players { get; protected set; }
        public IChunkMap Map { get; protected set; }

        public GameWorld(int maxPlayers, in ushort sizeX, in ushort sizeY)
        {
            SizeX = sizeX;
            SizeY = sizeY;
            Players = new WorldPlayers(maxPlayers);
            CreateMap();
        }

        public void FreeMemory()
        {
            foreach (var c in _preallocatedMap.AllChunks()) c.FreeMemoryForReuse();
        }

        public virtual void CreateMap()
        {
            _id = GameId.Generate();
            _preallocatedMap = new PreAllocatedChunkMap(this, SizeX, SizeY);
            _preallocatedMap.CreateMap(SizeX, SizeY);
            Map = _preallocatedMap;
            Log.Debug("Allocated Map");
        }

        public void Populate(int seed = 0, params ChunkPopulator[] populators)
        {
            if (seed == 0)
                seed = WorldUtils.Random.Next(0, ushort.MaxValue);
            WorldUtils.SetRandomSeed(seed);
            Seed = (ushort)seed;
            Log.Debug($"Generating world seed {seed} {SizeX}x{SizeY} for {Players.MaxPlayers} players");
            foreach (var chunk in _preallocatedMap.AllChunks())
                foreach (var populator in populators)
                    populator.Populate(this, _preallocatedMap, chunk);
        }

        public TileEntity GetUnusedStartingTile()
        {
            var freeChunk = _preallocatedMap.GetUnnocupiedNewbieChunk();
            if (freeChunk == null)
            {
                throw new Exception("No more room for newbie players in this world");
            }
            return freeChunk.FindTileWithId(0);
        }

        public virtual IEnumerable<TileEntity> AllTiles()
        {
            foreach (var chunk in _preallocatedMap.AllChunks())
                foreach (var tile in chunk.AllTiles())
                    yield return tile;
        }

        public Chunk GetTileChunk(in int tileX, in int tileY) => _preallocatedMap.GetTileChunk(tileX, tileY);
        public TileEntity GetTile(in Position pos) => _preallocatedMap.GetTile(pos.X, pos.Y);
        //public virtual TileEntity Map.GetTile(in int tileX, in int tileY) => Map.GetTile(tileX, tileY);
        public void Dispose() => FreeMemory();
    }
}
