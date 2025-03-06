using Game.Engine.DataTypes;
using Game.Entities;
using Game.Systems.Tile;
using Game.Tile;
using System.Collections.Generic;

namespace Game.World
{
    /// <summary>
    /// A chunk represents a small portion of the map tiles (e.g a 8x8 tile)
    /// All tile data of a given chunk is stored in the same memory pointer.
    /// </summary>
    public unsafe class Chunk 
    {
        private TileModel[,] _tileReferences;
        private Location _position;
        private IGameWorld _world;

        public IGameWorld World { get; }

        public Chunk(IGameWorld world, in int x, in int y)
        {
            World = world;
            _position = new Location(x, y);
            _tileReferences = new TileModel[GameWorld.CHUNK_SIZE, GameWorld.CHUNK_SIZE];
        }

        public ref readonly ushort X => ref _position.X;
        public ref readonly ushort Y => ref _position.Y;
        public ref readonly Location Position => ref _position;
        public TileModel[,] Tiles { get => _tileReferences; private set => _tileReferences = value; }

        public TileModel CreateTile(in int internalTileX, in int internalTileY, GameId entityId = default)
        {
            var dataPointer = new TileDataComponent()
            {
                Position = new Location()
                {
                    X = (ushort)(X * GameWorld.CHUNK_SIZE + internalTileX),
                    Y = (ushort)(Y * GameWorld.CHUNK_SIZE + internalTileY),
                }
            };
            var tileEntity = World.Game.Entities.CreateEntity(EntityType.Tile, entityId: entityId);
            var tile = new TileModel(this, tileEntity);
            tile.Components.Save(dataPointer);
            Tiles[internalTileX, internalTileY] = tile;
            return tile;
        }


        public TileModel GetTile(in int x, in int y)
        {
            var tile = Tiles[x, y];
            if (tile == null && !World.Game.IsClientGame) tile = CreateTile(x, y);
            return tile;
        }

        public override string ToString() => $"<Chunk x={X} y={Y}>";

        public IEnumerable<TileModel> AllTiles()
        {
            for (var x = 0; x < _tileReferences.GetLength(0); x++)
                for (var y = 0; y < _tileReferences.GetLength(1); y++)
                    yield return GetTile(x, y);
        }
    }
}
