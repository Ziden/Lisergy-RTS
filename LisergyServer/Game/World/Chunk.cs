using Game.DataTypes;
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

    public unsafe class Chunk : IDisposable
    {
        private ChunkData _data;
        private TileEntity[,] _tiles;
        private GameId _id;

        public ChunkMap Map { get; private set; }

        public Chunk(ChunkMap map, in ushort x, in ushort y, TileEntity[,] tiles)
        {
            _id = GameId.Generate();
            _data = new ChunkData();
            _data.Position = new Position(x, y);
            _data.Allocate();
            Map = map;
            _tiles = tiles;
        }

        public bool IsVoid() => _tiles == null;
        public ref readonly ushort X => ref _data.Position.X;
        public ref readonly ushort Y => ref _data.Position.Y;
        public ref readonly Position Position => ref _data.Position;
        public ref readonly byte Flags { get => ref _data._flags; }
        public void SetFlag(in byte flag) => _data.SetFlag(flag);
        public TileEntity[,] Tiles { get => _tiles; private set => _tiles = value; }
        public ref readonly GameId EntityId => ref _id;
        public void FreeMemoryForReuse() => _data.FlagToBeReused();

        public TileEntity CreateTile(in int tileX, in int tileY)
        {
            var internalTileX = tileX % GameWorld.CHUNK_SIZE;
            var internalTileY = tileY % GameWorld.CHUNK_SIZE;
            var dataPointer = _data.GetTileData(internalTileX, internalTileY);
            var tile = new TileEntity(this, dataPointer, tileX, tileY);
            return tile;
        }

        public TileEntity GetTile(in int x, in int y) => Tiles[x, y];
        public override string ToString() => $"<Chunk x={X} y={Y}>";
        public IEnumerable<TileEntity> AllTiles()
        {
            for (var x = 0; x < _tiles.GetLength(0); x++)
                for (var y = 0; y < _tiles.GetLength(1); y++)
                    yield return _tiles[x, y];
        }
        public void Dispose() => _data.Free();
    }
}
