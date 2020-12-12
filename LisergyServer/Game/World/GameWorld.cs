using Game.Generator;
using Game.World;
using System;
using System.Collections.Generic;

namespace Game
{
    public class GameWorld
    {
        // Amount of tiles the chunk length
        public static readonly int CHUNK_SIZE = 8;

        // Bitshift needed to get the chunk of a given tile
        public static readonly int CHUNK_SIZE_BITSHIFT = CHUNK_SIZE.BitsRequired() - 1;

        // how many tiles is the area of a chunk
        public static readonly int TILES_IN_CHUNK = CHUNK_SIZE * CHUNK_SIZE;

        // how many chunks are "player reserved chunks" per player
        public static readonly int PLAYERS_CHUNKS = 2;

        private int _sizeX;
        private int _sizeY;
        private WorldListener _listener;

        public WorldPlayers Players { get; private set; }
        public ChunkMap ChunkMap { get; private set; }

        public GameWorld(int qtdPlayers)
        {
            var amtOfChunks = qtdPlayers * PLAYERS_CHUNKS;
            var amtOfTiles = amtOfChunks * TILES_IN_CHUNK;
            var arraySize = (int)Math.Ceiling(Math.Sqrt(amtOfTiles));
            var extraNeeded = CHUNK_SIZE - arraySize % CHUNK_SIZE;
            this._sizeX = arraySize + extraNeeded;
            this._sizeY = arraySize + extraNeeded;
            Players = new WorldPlayers(qtdPlayers);
            ChunkMap = new ChunkMap(this._sizeX / CHUNK_SIZE, this._sizeY / CHUNK_SIZE);
            _listener = new WorldListener(this);
        }

        public ushort Seed { get; set; }

        public int GetSize()
        {
            return this._sizeX;
        }

        public bool ValidCoords(int x, int y)
        {
            return x >= 0 && x < GetSize() && y >= 0 && y < GetSize();
        }

        public void AddPlayer(PlayerEntity player)
        {
            var newbieChunk = ChunkMap.GetUnnocupiedNewbieChunk();
            if (newbieChunk == null)
            {
                throw new Exception("No more room for newbie players in this world");
            }
            Players.Add(player);
            var castleTile = Worldgen.FindTileWithout(newbieChunk.Tiles, TerrainData.WATER);
            byte castleID = StrategyGame.Specs.InitialBuilding.Id;
            player.Build(castleID, castleTile);
            return;
        }

        public IEnumerable<Tile> AllTiles()
        {
            foreach (var chunk in ChunkMap.AllChunks())
                foreach (var tile in chunk.AllTiles())
                    yield return tile;
        }

        public Chunk GetChunk(int tileX, int tileY)
        {
            int chunkX = tileX >> CHUNK_SIZE_BITSHIFT;
            var chunkY = tileY >> CHUNK_SIZE_BITSHIFT;
            return ChunkMap.GetChunk(chunkX, chunkY);
        }

        public Tile GetTile(int tileX, int tileY)
        {
            var internalTileX = tileX % CHUNK_SIZE;
            var internalTileY = tileY % CHUNK_SIZE;
            return GetChunk(tileX, tileY).GetTile(internalTileX, internalTileY);
        }
    }
}
