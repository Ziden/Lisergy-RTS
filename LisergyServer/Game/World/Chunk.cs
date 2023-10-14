using Game.DataTypes;
using Game.Systems.Tile;
using Game.Tile;
using System;
using System.Collections.Generic;

namespace Game.World
{
    [Flags]
    public enum ChunkFlag : byte
    {
        NEWBIE_CHUNK = 0b00000001,
        OCCUPIED = 0b00000010
    }

    public interface IChunk
    {
        IChunkMap Map { get; }
        TileEntity[,] Tiles { get; }
    }

    /// <summary>
    /// A chunk represents a small portion of the map tiles (e.g a 8x8 tile)
    /// All tile data of a given chunk is stored in the same memory pointer.
    /// </summary>
    public unsafe class Chunk : IDisposable, IChunk
    {
        private ChunkData _data;
        private TileEntity[,] _tileReferences;
        private GameId _id;

        public IChunkMap Map { get; private set; }

        public Chunk(IChunkMap map, in ushort x, in ushort y)
        {
            _data = new ChunkData()
            {
                Position = new Position(x, y)
            };
            _id = new GameId(_data.Position);
            _tileReferences = new TileEntity[GameWorld.CHUNK_SIZE, GameWorld.CHUNK_SIZE];
            _data.Allocate();
            Map = map;
        }

        public ref readonly ushort X => ref _data.Position.X;
        public ref readonly ushort Y => ref _data.Position.Y;
        public ref readonly Position Position => ref _data.Position;
        public ref readonly byte Flags { get => ref _data._flags; }
        public void SetFlag(in byte flag) => _data.SetFlag(flag);
        public TileEntity[,] Tiles { get => _tileReferences; private set => _tileReferences = value; }
        public ref readonly GameId EntityId => ref _id;
        public void FreeMemoryForReuse() => _data.FlagToBeReused();

        public TileEntity CreateTile(in int internalTileX, in int internalTileY)
        {
            if (Tiles[internalTileX, internalTileY] != null) throw new Exception($"Tile already created {internalTileX} {internalTileY}");
            TileMapData* dataPointer = _data.GetTileData(internalTileX, internalTileY);
            var tile = new TileEntity(this, dataPointer, X * GameWorld.CHUNK_SIZE + internalTileX, Y * GameWorld.CHUNK_SIZE + internalTileY);
            Tiles[internalTileX, internalTileY] = tile;
            return tile;
        }

        public TileEntity GetTile(in int x, in int y) => Tiles[x, y];
        public override string ToString() => $"<Chunk x={X} y={Y}>";
        public IEnumerable<TileEntity> AllTiles()
        {
            for (var x = 0; x < _tileReferences.GetLength(0); x++)
                for (var y = 0; y < _tileReferences.GetLength(1); y++)
                    yield return _tileReferences[x, y];
        }
        public void Dispose() => _data.Free();
    }
}
