using System;
using System.Runtime.InteropServices;

namespace Game.World.Data
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
        public TileData* _chunkTiles;
        public byte _flags;
        public Position Position;

        public TileData* GetTileData(int x, int y) => _chunkTiles + x + y * GameWorld.CHUNK_SIZE;

        public void SetFlag(byte flag)
        {
            _flags |= (byte)flag;
        }

        public void Allocate()
        {
            var bytes = GameWorld.TILES_IN_CHUNK * sizeof(TileData);
            _chunkTiles = (TileData*)UnmanagedMemory.Alloc(bytes);
            UnmanagedMemory.SetZeros((IntPtr)_chunkTiles, bytes);
            Console.WriteLine($"Allocated {bytes}b for chunk {Position}");
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
