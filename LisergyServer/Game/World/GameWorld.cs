using Game.Systems.Player;
using Game.Systems.Tile;
using Game.Tile;
using Game.World;
using System;
using System.Collections.Generic;

namespace Game
{
    public interface IGameWorld { }

    public class GameWorld : IGameWorld
    {
        private string _id;

        // Amount of tiles the chunk length
        public const int CHUNK_SIZE = 8;

        // Bitshift needed to get the chunk of a given tile
        public static readonly int CHUNK_SIZE_BITSHIFT = CHUNK_SIZE.BitsRequired() - 1;

        // how many tiles is the area of a chunk
        public const int TILES_IN_CHUNK = CHUNK_SIZE * CHUNK_SIZE;

        // how many chunks are "player reserved chunks" per player
        public const int PLAYERS_CHUNKS = 2;

        public GameLogic Game { get; set; }
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
        }

        public void FreeMap()
        {
            foreach (var c in Map.AllChunks())
            {
                c.FreeMemoryForReuse();
                foreach (var t in c.AllTiles())
                {
                    Map.ClearTile(t);
                }
            }
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

        /// <summary>
        /// Finds a newbie chunk that is not used and returns a random TileEntity of it of the given
        /// spec id
        /// </summary>
        public TileEntity GetUnusedStartingTile()
        {
            var freeChunk = Map.GetUnnocupiedNewbieChunk();
            if (freeChunk.IsVoid())
            {
                throw new Exception("No more room for newbie players in this world");
            }
            return freeChunk.FindTileWithId(0);
        }

        public virtual void PlaceNewPlayer(PlayerEntity player, TileEntity t)
        {
            Players.Add(player);
            var castleID = GameLogic.Specs.InitialBuilding;
            player.Build(castleID, t);

            ushort initialUnit = GameLogic.Specs.InitialUnit;
            var unit = player.RecruitUnit(initialUnit);
            unit.Name = "Merlin";
            var party = player.GetParty(0);
            player.PlaceUnitInParty(unit, party);
            party.Tile = t.GetNeighbor(Direction.EAST);
            Log.Debug($"Placed new player in {t}");
            return;
        }

        public virtual IEnumerable<TileEntity> AllTiles()
        {
            foreach (var chunk in Map.AllChunks())
                foreach (var tile in chunk.AllTiles())
                    yield return tile;
        }

        public Chunk GetTileChunk(int tileX, int tileY)
        {
            return Map.GetTileChunk(tileX, tileY);
        }

        public TileEntity GetTile(int tileX, int tileY)
        {
            return Map.GetTile(tileX, tileY);
        }
    }
}
