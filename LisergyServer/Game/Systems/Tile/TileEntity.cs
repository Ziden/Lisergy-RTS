using Game.DataTypes;
using Game.ECS;
using Game.Network;
using Game.Systems.FogOfWar;
using Game.Systems.Player;
using Game.Systems.Tile;
using Game.World;
using System.Collections.Generic;

namespace Game.Tile
{
    public unsafe partial class TileEntity : IEntity
    {
        private TileMapData* _tileData;
        private Chunk _chunk;
        private GameId _id;
        public ComponentSet _components { get; private set; }
        public EntityType EntityType => EntityType.Tile;

        public TileEntity(Chunk c, in TileMapData* tileData, in int x, in int y)
        {
            _chunk = c;
            _tileData = tileData;
            _tileData->X = (ushort)x;
            _tileData->Y = (ushort)y;
            _id = new GameId(_tileData->Position);
            _components = new ComponentSet(this);
            DeltaFlags = new DeltaFlags(this);
        }

        public void SetFlag(DeltaFlag flag)
        {
            DeltaFlags.SetFlag(flag);
            foreach (var e in EntitiesIn) e.DeltaFlags.SetFlag(flag);
            if (_staticEntity is BaseEntity w) w.DeltaFlags.SetFlag(flag);
        }

        public ref Chunk Chunk => ref _chunk;
        public ref byte SpecId { get => ref _tileData->TileId; }
        public ref readonly float MovementFactor { get => ref this.GetSpec().MovementFactor; }
        public ref readonly Position Position => ref _tileData->Position;
        public ref readonly ushort Y { get => ref Position.Y; }
        public ref readonly ushort X { get => ref Position.X; }
        public IReadOnlyCollection<PlayerEntity> PlayersViewing => _components.Get<TileVisibility>().PlayersViewing;
        public IReadOnlyCollection<IEntity> EntitiesViewing => _components.Get<TileVisibility>().EntitiesViewing;
        public IReadOnlyList<IEntity> EntitiesIn => _components.Get<TileHabitants>().EntitiesIn;
        private IEntity _staticEntity => _components.Get<TileHabitants>().Building;
        public ref readonly GameId EntityId => ref _id;
        public IComponentSet Components => _components;
        public bool Passable => MovementFactor > 0;

        public PlayerEntity Owner => null;

        public ref readonly GameId OwnerID => ref GameId.ZERO;

        public IGame Game => this.Chunk.Map.World.Game;

        public IEntityLogic EntityLogic => Game.Logic.EntityLogic(this);

        public override string ToString()
        {
            return $"<Tile {X}-{Y} ID={SpecId}>";
        }

        public T Get<T>() where T : IComponent
        {
           return _components.Get<T>();
        }
    }
}
