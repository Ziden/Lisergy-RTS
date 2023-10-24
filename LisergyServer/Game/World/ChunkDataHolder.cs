using Game.Pathfinder;
using Game.Systems.Tile;
using System;
using System.Runtime.InteropServices;

namespace Game.World
{
    /// <summary>
    /// Responsible for manipulating chunk data
    /// </summary>
    public unsafe class ChunkDataHolder
    {
        private ChunkData* _dataPointer;

        public ChunkData* Pointer => _dataPointer;

        public void Allocate(int chunksTotal)
        {
            var bytesPerChunk = GameWorld.TILES_IN_CHUNK * sizeof(TileData);
            var totalSize = bytesPerChunk * chunksTotal;
            _dataPointer = (ChunkData*)UnmanagedMemory.Alloc(totalSize);
            UnmanagedMemory.SetZeros((IntPtr)_dataPointer, totalSize);
        }

        public void FlagToBeReused()
        {
            _dataPointer->Flags = 0;
            UnmanagedMemory.FlagMemoryToBeReused((IntPtr)_dataPointer);
        }

        public void Free()
        {
            _dataPointer->Flags = 0;
            UnmanagedMemory.Free((IntPtr)_dataPointer);
        }

        public TileData* GetTileData(in int x, in int y) => (TileData*)_dataPointer +x + y * GameWorld.CHUNK_SIZE;
        public ref Position Position => ref _dataPointer->Position;
        public ref readonly byte ChunkFlags => ref _dataPointer->Flags;
        public void SetFlag(in byte flag) => _dataPointer->Flags |= flag;

    }
}
