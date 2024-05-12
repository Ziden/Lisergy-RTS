using Game.ECS;
using Game.Network;
using Game.Tile;
using System;
using Game.Systems.Player;
using Game.Systems.MapPosition;
using Game.Engine.ECS;
using Game.Engine.DataTypes;
using Game.Engine.Network;

namespace Game
{
    public abstract partial class BaseEntity : IEntity
    {
        private GameId _entityId;
        private GameId _ownerId;

        public ref readonly GameId EntityId => ref _entityId;
        public ref readonly GameId OwnerID => ref _ownerId;
        public IComponentSet Components { get; private set; }
        public IGame Game { get; private set; }

        public BaseEntity(IGame game, GameId ownerId)
        {
            Game = game;
            _ownerId = ownerId;
            _entityId = GameId.Generate();
            DeltaFlags = new DeltaFlags(this);
            Components = new ComponentSet(this);
        }

        public TileEntity Tile => Components.GetReference<MapReferenceComponent>().Tile;
        public IEntityLogic EntityLogic => Game.Logic.GetEntityLogic(this);
        public abstract EntityType EntityType { get; }
        public ref T Get<T>() where T : unmanaged, IComponent => ref Components.Get<T>();
        public void Save<T>(in T component) where T : unmanaged, IComponent => Components.Save(component);
    }
}
