using Game.Systems.Player;
using Game.Systems.Tile;
using Game.Tile;
using System;
using System.Collections.Generic;

namespace Game.World
{
    public interface IGameWorld {
        public IGamePlayers Players { get; }
        public TileEntity GetTile(in int x, in int y);
    }

    public class GameWorld : IGameWorld
    {
        private string _id;
        public const int CHUNK_SIZE = 8;
        public static readonly int CHUNK_SIZE_BITSHIFT = CHUNK_SIZE.BitsRequired() - 1;
        public const int TILES_IN_CHUNK = CHUNK_SIZE * CHUNK_SIZE;
        public const int PLAYERS_CHUNKS = 2;
        public virtual IGame Game { get; set; }

        // TODO: Move out of world
        public WorldPlayers _worldPlayers { get; set; }
        public ChunkMap Map { get; set; }
        public ushort Seed { get; set; }
        public ushort SizeX { get; private set; }
        public ushort SizeY { get; private set; }

        public IGamePlayers Players => _worldPlayers;

        public GameWorld(int maxPlayers, in ushort sizeX, in ushort sizeY)
        {
            SizeX = sizeX;
            SizeY = sizeY;
            _worldPlayers = new WorldPlayers(maxPlayers);
        }

        public void FreeMemory()
        {
            foreach (var c in Map.AllChunks())
            {
                c.FreeMemoryForReuse();
                foreach (var t in c.AllTiles())
                {
                    Map.ClearTile(t);
                }
            }
            _worldPlayers.Free();
        }

        public virtual void AllocateMemory()
        {
            _id = Guid.NewGuid().ToString();
            Map = new ChunkMap(this, SizeX, SizeY);
            GenerateTiles();
        }

        public void Populate(int seed = 0, params ChunkPopulator[] populators)
        {
            if (seed == 0)
                seed = WorldUtils.Random.Next(0, ushort.MaxValue);
            WorldUtils.SetRandomSeed(seed);
            Seed = (ushort)seed;
            Log.Debug($"Generating world seed {seed} {SizeX}x{SizeY} for {Players.MaxPlayers} players");
            foreach (var chunk in Map.AllChunks())
                foreach (var populator in populators)
                    populator.Populate(this, chunk);
        }

        public virtual void GenerateTiles()
        {
            Map.GenerateTiles(SizeX, SizeY);
        }

        public TileEntity GetUnusedStartingTile()
        {
            var freeChunk = Map.GetUnnocupiedNewbieChunk();
            if (freeChunk.IsVoid())
            {
                throw new Exception("No more room for newbie players in this world");
            }
            return freeChunk.FindTileWithId(0);
        }

        public virtual IEnumerable<TileEntity> AllTiles()
        {
            foreach (var chunk in Map.AllChunks())
                foreach (var tile in chunk.AllTiles())
                    yield return tile;
        }

        public Chunk GetTileChunk(in int tileX, in int tileY)
        {
            return Map.GetTileChunk(tileX, tileY);
        }

        public TileEntity GetTile(in int tileX, in int tileY)
        {
            return Map.GetTile(tileX, tileY);
        }
    }
}
