using Game.Tile;
using System;
using System.Collections.Generic;

namespace Game.World
{
    public interface IGameWorld : IChunkMap, IDisposable
    {
        /// <summary>
        /// Gets the game this world belongs to
        /// </summary>
        public IGame Game { get; }

        /// <summary>
        /// Gets the players that reside in this world
        /// </summary>
        public IGamePlayers Players { get; } // TODO: Should players and entities be here or not ? CONFUSING !

        /// <summary>
        /// Populates the given world using the chunk populators configured
        /// </summary>
        public void Populate();
        TileModel GetUnusedStartingTile();
    }

    public class GameWorld : IGameWorld
    {
        public const int CHUNK_SIZE = 8;
        public const int TILES_IN_CHUNK = CHUNK_SIZE * CHUNK_SIZE;
        public const int PLAYERS_CHUNKS = 2;
        public static readonly int CHUNK_SIZE_BITSHIFT = CHUNK_SIZE.BitsRequired() - 1;

        public List<ChunkPopulator> ChunkPopulators { get; private set; } = new List<ChunkPopulator>();
        public virtual IGame Game { get; set; }
        public int Seed { get; set; }
        public ushort SizeX { get; private set; }
        public ushort SizeY { get; private set; }
        public IGamePlayers Players { get; protected set; }
        public ServerChunkMap Chunks { get; protected set; }
        public (int x, int y) TilemapDimensions => Chunks.TilemapDimensions;
        public (int x, int y) ChunkMapDimensions => Chunks.ChunkMapDimensions;

        public GameWorld(IGame game, in ushort sizeX, in ushort sizeY)
        {
            SizeX = sizeX;
            SizeY = sizeY;
            Players = new WorldPlayers(int.MaxValue);
            Game = game;
            CreateMap();
        }

        public virtual void CreateMap()
        {
            Chunks = new ServerChunkMap(this, SizeX, SizeY);
            Chunks.CreateMap(SizeX, SizeY);
        }

        public void Populate()
        {
            if (ChunkPopulators.Count == 0) return;
            if (Seed == 0)
                Seed = WorldUtils.Random.Next(0, ushort.MaxValue);
            WorldUtils.SetRandomSeed(Seed);
            Game.Log.Debug($"Populating world seed {Seed} {SizeX}x{SizeY} for {Players.MaxPlayers} players");
            foreach (var chunk in Chunks.AllChunks())
                foreach (var populator in ChunkPopulators)
                    populator.Populate(this, Chunks, chunk);
        }

        public TileModel GetUnusedStartingTile()
        {
            var freeChunk = Chunks.GetUnnocupiedNewbieChunk();
            if (freeChunk == null)
            {
                throw new Exception("No more room for newbie players in this world");
            }
            return freeChunk.FindTileWithId(0);
        }


        public virtual IEnumerable<TileModel> AllTiles()
        {
            foreach (var chunk in Chunks.AllChunks())
                foreach (var tile in chunk.AllTiles())
                    yield return tile;
        }

        public void Dispose() { }
        public bool ValidCoords(in int tileX, in int tileY) => Chunks.ValidCoords(tileX, tileY);
        public Chunk GetChunk(in int chunkX, in int chunkY) => Chunks.GetChunk(chunkX, chunkY);
        public IEnumerable<Location> FindPath(TileModel from, TileModel to) => Chunks.FindPath(from, to);
        public void CreateMap(in ushort sizeX, in ushort sizeY) => Chunks.CreateMap(sizeX, sizeY);
        public TileModel GetTile(in int tileX, in int tileY) => Chunks.GetTile(tileX, tileY);
        public TileModel GetTile(in Location p) => Chunks.GetTile(p);
        public Chunk GetTileChunk(in Location p) => Chunks.GetTileChunk(p);
    }
}
