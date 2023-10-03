using Game.Pathfinder;
using Game.Systems.Tile;
using System;
using System.Runtime.InteropServices;

namespace Game.World
{
    /// <summary>
    /// Unmanaged memory to allocate map data.
    /// Allocates a chunk of tiles.
    /// </summary>
    // TODO: REMOVE CHUNKS
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct ChunkData
    {
        public TileMapData* _chunkTiles;
        public byte _flags;
        public MapPosition Position;

        public TileMapData* GetTileData(int x, int y) => _chunkTiles + x + y * GameWorld.CHUNK_SIZE;

        public void SetFlag(byte flag)
        {
            _flags |= flag;
        }

        public void Allocate()
        {
            var bytes = GameWorld.TILES_IN_CHUNK * sizeof(TileMapData);
            _chunkTiles = (TileMapData*)UnmanagedMemory.Alloc(bytes);
            UnmanagedMemory.SetZeros((IntPtr)_chunkTiles, bytes);
        }

        public void FlagToBeReused()
        {
            _flags = 0;
            UnmanagedMemory.FlagMemoryToBeReused((IntPtr)_chunkTiles);
        }

        public void Free()
        {
            _flags = 0;
            UnmanagedMemory.Free((IntPtr)_chunkTiles);
        }
    }
}
