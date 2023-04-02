using Game;
using Game.Tile;
using Game.World;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        public override TileEntity GetTile(int tileX, int tileY)
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

        public override ref Chunk GetTileChunk(int tileX, int tileY)
        {
            var chunk = base.GetTileChunk(tileX, tileY);
            if (chunk == null)
            {
                int chunkX = tileX.ToChunkCoordinate();
                var chunkY = tileY.ToChunkCoordinate();
                var newChunk = new Chunk(this, chunkX, chunkY, new TileEntity[GameWorld.CHUNK_SIZE, GameWorld.CHUNK_SIZE]);
                Log.Debug($"Allocating Chunk {newChunk}");
                this.Add(ref newChunk);
            }
            return ref base.GetTileChunk(tileX, tileY);
        }
    }
}
