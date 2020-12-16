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
            int chunkX = tileX.ToChunkCoordinate();
            var chunkY = tileY.ToChunkCoordinate();
            var chunk = GetChunk(chunkX, chunkY);
            var internalX = tileX % CHUNK_SIZE;
            var internalY = tileY % CHUNK_SIZE;
            var tile = chunk.GetTile(internalX, internalY);
            if (tile == null)
            {
                tile = new ClientTile((ClientChunk)chunk, tileX, tileY);
                chunk.Tiles[internalX, internalY] = tile;
                Log.Debug($"Created {tile}");
            }
            return tile;

        }

        public override Chunk GetChunk(int tileX, int tileY)
        {
            var chunk = base.GetChunk(tileX, tileY);
            if (chunk == null)
            {
                int chunkX = tileX.ToChunkCoordinate();
                var chunkY = tileY.ToChunkCoordinate();
                chunk = new ClientChunk(this, chunkX, chunkY);
                this.ChunkMap.Add(chunk);
            }
            return chunk;
        }

    }
}
