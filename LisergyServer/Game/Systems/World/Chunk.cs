using Game.DataTypes;
using Game.ECS;
using Game.Events;
using Game.Events.ServerEvents;
using Game.Network;
using Game.Pathfinder;
using Game.Systems.Player;
using Game.Tile;
using System;
using System.Collections.Generic;

namespace Game.Systems.World
{
    [Flags]
    public enum ChunkFlag : byte
    {
        NEWBIE_CHUNK = 0b00000001,
        OCCUPIED = 0b00000010
    }

    public unsafe class Chunk : IEntity, IDisposable
    {
        private ChunkData _data;
        private TileEntity[,] _tiles;
        private GameId _id;

        public bool IsVoid()
        {
            return _tiles == null;
        }

        public ushort X => _data.Position.X;
        public ushort Y => _data.Position.Y;
        public MapPosition Position => _data.Position;

        public ChunkMap Map { get; private set; }
        public byte Flags { get => _data._flags; }

        public void SetFlag(byte flag) => _data.SetFlag(flag);

        public TileEntity[,] Tiles { get => _tiles; private set => _tiles = value; }

        public IComponentSet Components => throw new NotImplementedException();

        public GameId EntityId => _id;

        public PlayerEntity Owner => throw new NotImplementedException();

        public GameId OwnerID => throw new NotImplementedException();

        public Chunk(ChunkMap w, int x, int y, TileEntity[,] tiles)
        {
            ;
            _id = GameId.Generate();
            _data = new ChunkData();
            _data.Position = new MapPosition(x, y);
            _data.Allocate();
            Map = w;
            _tiles = tiles;
        }

        public void FreeMemoryForReuse()
        {
            _data.FlagToBeReused();
        }

        public TileEntity CreateTile(in int tileX, in int tileY)
        {
            var internalTileX = tileX % GameWorld.CHUNK_SIZE;
            var internalTileY = tileY % GameWorld.CHUNK_SIZE;
            var dataPointer = _data.GetTileData(internalTileX, internalTileY);
            var tile = new TileEntity(this, dataPointer, tileX, tileY);
            return tile;
        }

        public TileEntity GetTile(in int x, in int y)
        {
            return Tiles[x, y];
        }

        public override string ToString()
        {
            return $"<Chunk x={X} y={Y}>";
        }

        public IEnumerable<TileEntity> AllTiles()
        {
            for (var x = 0; x < _tiles.GetLength(0); x++)
                for (var y = 0; y < _tiles.GetLength(1); y++)
                    yield return _tiles[x, y];
        }

        public void Dispose()
        {
            _data.Free();
        }

        public T Get<T>() where T : IComponent
        {
            throw new NotImplementedException();
        }

        ServerPacket IDeltaUpdateable.GetUpdatePacket(PlayerEntity receiver)
        {
            throw new NotImplementedException();
        }
    }
}
