using Game.Generator;
using Game.World;
using System;
using System.Collections.Generic;

namespace Game
{
    public class GameWorld
    {
        private string _id;

        // Amount of tiles the chunk length
        public static readonly int CHUNK_SIZE = 8;

        // Bitshift needed to get the chunk of a given tile
        public static readonly int CHUNK_SIZE_BITSHIFT = CHUNK_SIZE.BitsRequired() - 1;

        // how many tiles is the area of a chunk
        public static readonly int TILES_IN_CHUNK = CHUNK_SIZE * CHUNK_SIZE;

        // how many chunks are "player reserved chunks" per player
        public static readonly int PLAYERS_CHUNKS = 2;

        public WorldPlayers Players { get; set; }
        public ChunkMap ChunkMap { get; set; }

        public ushort Seed { get; set; }
        public int SizeX { get; private set; }
        public int SizeY { get; private set; }

        public GameWorld(int maxPlayers, int sizeX, int sizeY)
        {
            SizeX = sizeX;
            SizeY = sizeY;
            Players = new WorldPlayers(maxPlayers);
            CreateChunkMap();
        }

        public virtual void CreateChunkMap()
        {
            _id = Guid.NewGuid().ToString();
            ChunkMap = new ChunkMap(this);
            GenerateTiles();
        }

        public virtual void GenerateTiles()
        {
            var maxChunkX = this.SizeX >> GameWorld.CHUNK_SIZE_BITSHIFT;
            var maxChunkY = this.SizeY >> GameWorld.CHUNK_SIZE_BITSHIFT;
            for (var chunkX = 0; chunkX < maxChunkX; chunkX++)
            {
                for (var chunkY = 0; chunkY < maxChunkY; chunkY++)
                {
                    var tiles = new Tile[GameWorld.CHUNK_SIZE, GameWorld.CHUNK_SIZE];
                    var chunk = new Chunk(ChunkMap, chunkX, chunkY, tiles);
                    for (var x = 0; x < GameWorld.CHUNK_SIZE; x++)
                    {
                        for (var y = 0; y < GameWorld.CHUNK_SIZE; y++)
                        {
                            var tileX = chunkX * GameWorld.CHUNK_SIZE + x;
                            var tileY = chunkY * GameWorld.CHUNK_SIZE + y;
                            tiles[x, y] = new Tile(chunk, tileX, tileY);
                        }
                    }
                    this.ChunkMap.Add(chunk);
                }
            }
        }

        public virtual void PlaceNewPlayer(PlayerEntity player, Tile t = null)
        {
            var newbieChunk = ChunkMap.GetUnnocupiedNewbieChunk();
            if (newbieChunk == null)
            {
                throw new Exception("No more room for newbie players in this world");
            }
            Players.Add(player);
            if(t == null)
            {
                t = newbieChunk.FindTileWithId(0);
            }
            byte castleID = StrategyGame.Specs.InitialBuilding;
            player.Build(castleID, t);

            ushort initialUnit = StrategyGame.Specs.InitialUnit;
            var unit = player.RecruitUnit(initialUnit);

            var party = player.Parties[0];
            player.PlaceUnitInParty(unit, party);
            player.DeployParty(party, t.GetNeighbor(Direction.EAST));
            Log.Debug($"Placed new player in {t}");
            return;
        }

        public virtual IEnumerable<Tile> AllTiles()
        {
            foreach (var chunk in ChunkMap.AllChunks())
                foreach (var tile in chunk.AllTiles())
                    yield return tile;
        }

        public Chunk GetTileChunk(int tileX, int tileY)
        {
            return ChunkMap.GetTileChunk(tileX, tileY);
        }

        public Tile GetTile(int tileX, int tileY)
        {
            return ChunkMap.GetTile(tileX, tileY);
        }
    }
}
