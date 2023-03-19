using Game.DataTypes;
using Game.ECS;
using Game.Entity;
using Game.Events;
using Game.Events.ServerEvents;
using Game.World.Components;
using Game.World.Data;
using System;
using System.Collections.Generic;
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
        public ComponentSet<Tile> _components { get; private set; }

        public Tile(Chunk c, TileData* tileData, int x, int y)
        {
            _chunk = c;
            _tileData = tileData;
            _tileData->X = (ushort)x;
            _tileData->Y = (ushort)y;
            _components = new ComponentSet<Tile>(this);
            DeltaFlags = new DeltaFlags(this);
        }

        public void SetFlagIncludingChildren(DeltaFlag flag)
        {
            DeltaFlags.SetFlag(flag);
            foreach (var e in EntitiesIn) e.DeltaFlags.SetFlag(flag);
            if (_staticEntity != null) _staticEntity.DeltaFlags.SetFlag(flag);
        }

        public ref Chunk Chunk => ref _chunk;
        public byte TileId { get => _tileData->TileId; set => _tileData->TileId = value; }
        public float MovementFactor { get => this.GetSpec().MovementFactor; }
        public ushort Y { get => _tileData->Y; set => _tileData->Y = value; }
        public ushort X { get => _tileData->X; set => _tileData->X = value; }
        public IReadOnlyCollection<PlayerEntity> PlayersViewing => _components.Get<TileVisibility>().PlayersViewing;
        public IReadOnlyCollection<WorldEntity> EntitiesViewing => _components.Get<TileVisibility>().EntitiesViewing;
        public IReadOnlyList<WorldEntity> EntitiesIn => _components.Get<TileHabitants>().EntitiesIn;
        private BuildingEntity _staticEntity => _components.Get<TileHabitants>().Building;
        public GameId TileUniqueId => new GameId(_tileData->Position);



        public IComponentSet Components => _components;

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
