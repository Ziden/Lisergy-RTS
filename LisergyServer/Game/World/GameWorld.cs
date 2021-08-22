
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

        public StrategyGame Game { get; set; }
        public WorldPlayers Players { get; set; }
        public ChunkMap Map { get; set; }

        public ushort Seed { get; set; }
        public int SizeX { get; private set; }
        public int SizeY { get; private set; }

        public GameWorld(int maxPlayers, int sizeX, int sizeY)
        {
            SizeX = sizeX;
            SizeY = sizeY;
            Players = new WorldPlayers(maxPlayers);
            CreateMap();
        }

        public virtual void CreateMap()
        {
            _id = Guid.NewGuid().ToString();
            Map = new ChunkMap(this, SizeX, SizeY);
            GenerateTiles();
        }

        public virtual void GenerateTiles()
        {
            this.Map.GenerateTiles(this.SizeX, this.SizeY);
        }

        public virtual void PlaceNewPlayer(PlayerEntity player, Tile t = null)
        {
            var newbieChunk = Map.GetUnnocupiedNewbieChunk();
            if (newbieChunk == null)
            {
                throw new Exception("No more room for newbie players in this world");
            }
            Players.Add(player);
            if(t == null)
            {
                t = newbieChunk.FindTileWithId(0);
            }
            var castleID = StrategyGame.Specs.InitialBuilding;
            player.Build(castleID, t);

            ushort initialUnit = StrategyGame.Specs.InitialUnit;
            var unit = player.RecruitUnit(initialUnit);
            unit.Name = "Merlin";
            var party = player.GetParty(0);
            player.PlaceUnitInParty(unit, party);
            party.Tile =  t.GetNeighbor(Direction.EAST);
            Log.Debug($"Placed new player in {t}");
            return;
        }

        public virtual IEnumerable<Tile> AllTiles()
        {
            foreach (var chunk in Map.AllChunks())
                foreach (var tile in chunk.AllTiles())
                    yield return tile;
        }

        public Chunk GetTileChunk(int tileX, int tileY)
        {
            return Map.GetTileChunk(tileX, tileY);
        }

        public Tile GetTile(int tileX, int tileY)
        {
            return Map.GetTile(tileX, tileY);
        }
    }
}
