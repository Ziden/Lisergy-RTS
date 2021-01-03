using Game;
using Game.Entity;
using Game.World;
using System.Collections.Generic;

namespace Assets.Code.World
{
    public class ClientWorld : GameWorld
    {
        public Dictionary<string, Party> Parties = new Dictionary<string, Party>();

        public ClientPlayer GetOrCreateClientPlayer(string uid)
        {
            PlayerEntity pl;
            if (Players.GetPlayer(uid, out pl))
                return (ClientPlayer)pl;
            if (uid == MainBehaviour.Player.UserID)
            {
                Players.Add(MainBehaviour.Player);
                return MainBehaviour.Player;
            }
            pl = new ClientPlayer();
            pl.UserID = uid;
            Players.Add(pl);
            return (ClientPlayer)pl;
        }

        public override Tile GetTile(int tileX, int tileY)
        {
            if (!ValidCoords(tileX, tileY))
            {
                StackLog.Debug($"Invalid coords {tileX}-{tileY}");
                return null;
            }
            var tile = base.GetTile(tileX, tileY);
            if (tile == null)
            {
                StackLog.Debug($"Creating tile {tileX} {tileY}");
                var chunk = base.GetTileChunk(tileX, tileY);
                tile = new ClientTile((ClientChunk)chunk, tileX, tileY);
                chunk.Tiles[tileX % GameWorld.CHUNK_SIZE, tileY % GameWorld.CHUNK_SIZE] = tile;

            }
            return tile;
        }

        public override Chunk GetTileChunk(int tileX, int tileY)
        {
            var chunk = base.GetTileChunk(tileX, tileY);
            if (chunk == null)
            {
                int chunkX = tileX.ToChunkCoordinate();
                var chunkY = tileY.ToChunkCoordinate();
                chunk = new ClientChunk(this, chunkX, chunkY);
                this.ChunkMap.Add(chunk);
                StackLog.Debug($"Created {chunk}");
            }
            return chunk;
        }

    }
}
