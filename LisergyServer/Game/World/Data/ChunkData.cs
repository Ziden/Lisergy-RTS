using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Game.World.Data
{
    /// <summary>
    /// Unmanaged memory to allocate map data.
    /// Allocates a chunk of tiles.
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct ChunkData
    {
        public TileData[] _chunkTiles;
        public byte _flags;
        public Position Position;

        public ref TileData GetTileData(int x, int y)
        {
            return ref _chunkTiles[x + y * GameWorld.CHUNK_SIZE];
        }

        public void SetFlag(byte flag)
        {
            _flags |= (byte)flag;
        }


        public void Allocate()
        {
            var amountTilesTotal = GameWorld.TILES_IN_CHUNK;
            var bytes = amountTilesTotal * sizeof(TileData);
            _chunkTiles = new TileData[GameWorld.CHUNK_SIZE * GameWorld.CHUNK_SIZE];
            Console.WriteLine($"Allocated {bytes} bytes tiles in chunk {Position}");
            for (var x = 0; x < GameWorld.CHUNK_SIZE; x++)
            {
                for (var y = 0; y < GameWorld.CHUNK_SIZE; y++)
                {
                    _chunkTiles[x + y * GameWorld.CHUNK_SIZE] = new TileData();
                }
            }
        }
    }
}
