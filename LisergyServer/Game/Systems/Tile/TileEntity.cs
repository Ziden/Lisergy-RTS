using Game.DataTypes;
using Game.ECS;
using Game.Events.ServerEvents;
using Game.Network;
using Game.Player;
using Game.Systems.FogOfWar;
using Game.Systems.Tile;
using System;
using System.Collections.Generic;

namespace Game.Tile
{
    [Serializable]
    public unsafe partial class TileEntity : IEntity, IDeltaTrackable, IDeltaUpdateable<TileUpdatePacket>
    {
        [NonSerialized]
        private TileData* _tileData;

        [NonSerialized]
        private Chunk _chunk;

        // Only unique game id among tiles, non unique entity id
        [NonSerialized]
        private GameId _id;

        [field: NonSerialized]
        public ComponentSet _components { get; private set; }

        public TileEntity(Chunk c, TileData* tileData, int x, int y)
        {
            _chunk = c;
            _tileData = tileData;
            _tileData->X = (ushort)x;
            _tileData->Y = (ushort)y;
            _id = new GameId(_tileData->Position);
            _components = new ComponentSet(this);
            DeltaFlags = new DeltaFlags(this);
        }

        public void SetFlagIncludingChildren(DeltaFlag flag)
        {
            DeltaFlags.SetFlag(flag);
            foreach (var e in EntitiesIn) e.DeltaFlags.SetFlag(flag);
            if (_staticEntity is WorldEntity w) w.DeltaFlags.SetFlag(flag);
        }

        public ref Chunk Chunk => ref _chunk;
        public byte TileId { get => _tileData->TileId; set => _tileData->TileId = value; }
        public float MovementFactor { get => this.GetSpec().MovementFactor; }
        public ushort Y { get => _tileData->Y; set => _tileData->Y = value; }
        public ushort X { get => _tileData->X; set => _tileData->X = value; }
        public IReadOnlyCollection<PlayerEntity> PlayersViewing => _components.Get<TileVisibility>().PlayersViewing;
        public IReadOnlyCollection<IEntity> EntitiesViewing => _components.Get<TileVisibility>().EntitiesViewing;
        public IReadOnlyList<WorldEntity> EntitiesIn => _components.Get<TileHabitants>().EntitiesIn;
        private IEntity _staticEntity => _components.Get<TileHabitants>().Building;
        public GameId EntityId => _id;
        public IComponentSet Components => _components;
        public StrategyGame Game => Chunk.Map.World.Game;
        public bool Passable
        {
            get => MovementFactor > 0;
        }

        public PlayerEntity Owner => Gaia.Instance;

        public GameId OwnerID => Owner.OwnerID;

        public override string ToString()
        {
            return $"<Tile {X}-{Y} ID={TileId}>";
        }
    }
}
