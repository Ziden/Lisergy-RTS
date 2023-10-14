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
            SetupComponents();
        }

        public void SetupComponents()
        {
            Components.Add<TileComponent>();
            Components.AddReference(new TileVisibility());
            Components.AddReference(new TileHabitants());
        }

        public void SetFlag(DeltaFlag flag)
        {
            DeltaFlags.SetFlag(flag);
            foreach (var e in EntitiesIn) e.DeltaFlags.SetFlag(flag);
            Building?.DeltaFlags.SetFlag(flag);
        }

        public void UpdateData(in TileMapData newData) => *_tileData = newData;
        public ref Chunk Chunk => ref _chunk;
        public ref byte SpecId { get => ref _tileData->TileId; }
        public ref readonly float MovementFactor { get => ref this.GetSpec().MovementFactor; }
        public ref readonly Position Position => ref _tileData->Position;
        public ref readonly ushort Y { get => ref Position.Y; }
        public ref readonly ushort X { get => ref Position.X; }
        public IReadOnlyCollection<PlayerEntity> PlayersViewing => _components.GetReference<TileVisibility>().PlayersViewing;
        public IReadOnlyCollection<IEntity> EntitiesViewing => _components.GetReference<TileVisibility>().EntitiesViewing;
        public IReadOnlyList<IEntity> EntitiesIn => _components.GetReference<TileHabitants>().EntitiesIn;
        public IEntity Building => _components.GetReference<TileHabitants>().Building;
        public ref readonly GameId EntityId => ref _id;
        public IComponentSet Components => _components;
        public bool Passable => MovementFactor > 0;
        public PlayerEntity Owner => null;
        public ref readonly GameId OwnerID => ref GameId.ZERO;
        public IGame Game => this.Chunk.Map.World.Game;
        public IEntityLogic EntityLogic => Game.Logic.GetEntityLogic(this);

        public override string ToString() => $"<Tile Type={SpecId} {Position}>";
        public ref T Get<T>() where T : unmanaged, IComponent => ref _components.Get<T>();
    }
}
