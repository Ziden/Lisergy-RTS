using Game;
using Game.World;
using Game.World.Data;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Assets.Code.World
{
    /// <summary>
    /// Generate tiles on demands (empty tiles).
    /// Will wait for server to send data to instantiate.
    /// </summary>
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
                tile = GenerateTile(ref chunk, tileX, tileY);
                //tile = new Tile(chunk, CreateTileDataPointer(), tileX, tileY);
                chunk.Tiles[tileX % GameWorld.CHUNK_SIZE, tileY % GameWorld.CHUNK_SIZE] = tile;
            }
            return tile;
        }

        public override Chunk GetTileChunk(int tileX, int tileY)
        {
            var chunk = base.GetTileChunk(tileX, tileY);
            if (chunk.HasValue())
            {
                int chunkX = tileX.ToChunkCoordinate();
                var chunkY = tileY.ToChunkCoordinate();
                chunk = new Chunk(this, chunkX, chunkY, new Tile[GameWorld.CHUNK_SIZE, GameWorld.CHUNK_SIZE]);
                this.Add(chunk);
                StackLog.Debug($"Created {chunk}");
            }
            return chunk;
        }
    }
}
