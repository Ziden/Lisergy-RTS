using Game.ECS;
using Game.Entity;
using Game.Events;
using Game.Events.ServerEvents;
using Game.World.Components;
using Game.World.Data;
using System;
using System.Runtime.InteropServices;

namespace Game
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public unsafe partial class Tile : IEntity, IDeltaTrackable, IDeltaUpdateable<TileUpdatePacket>
    {
        [NonSerialized]
        private TileData* _tileData;

        [NonSerialized]
        private Chunk _chunk;

        [field: NonSerialized]
        public ComponentSet<Tile> Components { get; private set; }

        public Tile(Chunk c, TileData* tileData, int x, int y)
        {
            _chunk = c;
            _tileData = tileData;
            _tileData->X = (ushort)x;
            _tileData->Y = (ushort)y;
            Components = new ComponentSet<Tile>(this);
            DeltaFlags = new DeltaFlags(this);
        }

        public ref Chunk Chunk => ref _chunk;
        public byte TileId { get => _tileData->TileId; set => _tileData->TileId = value; }
        public float MovementFactor { get => this.GetSpec().MovementFactor; }
        public ushort Y { get => _tileData->Y; }
        public ushort X { get => _tileData->X; }

        public GameId TileUniqueId => new GameId(_tileData->Position);

        public T AddComponent<T>() where T : IComponent => Components.AddComponent<T>();
        public T GetComponent<T>() where T : IComponent => Components.Get<T>();

        public StrategyGame Game => Chunk.Map.World.Game;

        public bool Passable
        {
            get => MovementFactor > 0;
        }

        public override string ToString()
        {
            return $"<Tile {X}-{Y} ID={TileId}>";
        }
    }
}
