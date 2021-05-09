using Game;
using Game.World;

namespace Assets.Code.World
{
    public class ClientChunkMap : ChunkMap
    {
        public ClientChunkMap(ClientWorld world) : base(world, world.SizeX, world.SizeY) { }

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
                this.Add(chunk);
                StackLog.Debug($"Created {chunk}");
            }
            return chunk;
        }
    }
}
