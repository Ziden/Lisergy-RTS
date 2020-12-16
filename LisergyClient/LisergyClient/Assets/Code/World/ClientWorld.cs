using Game;
using Game.World;

namespace Assets.Code.World
{
    public class ClientWorld : GameWorld
    {
        public ClientWorld(): base ()
        {

        }

        public override WorldListener GetListener()
        {
            return new ClientWorldListener(this);
        }

        public ClientPlayer GetClientPlayer(string uid)
        {
            PlayerEntity pl;
            if (Players.GetPlayer(uid, out pl))
                return (ClientPlayer)pl;
            pl = new ClientPlayer();
            pl.UserID = uid;
            Players.Add(pl);
            return (ClientPlayer)pl;
        }

        public override Tile GetTile(int tileX, int tileY)
        {
            if (!ValidCoords(tileX, tileY))
            {
                Log.Debug($"Invalid coords {tileX}-{tileY}");
                return null;
            }
            var tile = base.GetTile(tileX, tileY);
            if (tile == null)
            {
                var chunk = base.GetTileChunk(tileX, tileY);
                tile = new ClientTile((ClientChunk)chunk, tileX, tileY);
                chunk.Tiles[tileX % GameWorld.CHUNK_SIZE, tileY % GameWorld.CHUNK_SIZE] = tile;
                Log.Debug($"Created {tile}");
            }
            return tile;
        }

        public override Chunk GetTileChunk(int tileX, int tileY)
        {
            Log.Debug($"Get chunk {tileX} {tileY}");
            var chunk = base.GetTileChunk(tileX, tileY);
            if (chunk == null)
            {
                int chunkX = tileX.ToChunkCoordinate();
                var chunkY = tileY.ToChunkCoordinate();
                chunk = new ClientChunk(this, chunkX, chunkY);
                this.ChunkMap.Add(chunk);
                Log.Debug($"Created {chunk}");
            }
            return chunk;
        }

    }
}
