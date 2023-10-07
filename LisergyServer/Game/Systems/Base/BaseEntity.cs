using Game.DataTypes;
using Game.ECS;
using Game.Network;
using Game.Tile;
using System;
using Game.Systems.Player;
using Game.Systems.MapPosition;

namespace Game
{
    [Serializable] // TODO: Make it not serializable
    public abstract partial class BaseEntity : IEntity
    {
        public GameId EntityId { get; private set; }
        public GameId OwnerID { get; private set; }

        [field: NonSerialized] public IComponentSet Components { get; private set; }
        [field: NonSerialized] public IGame Game { get; private set; }

        public BaseEntity(IGame game, PlayerEntity owner)
        {
            Game = game;
            OwnerID = owner != null ? owner.OwnerID : GameId.ZERO;    
            EntityId = Guid.NewGuid();
            DeltaFlags = new DeltaFlags(this);
            Components = new ComponentSet(this, owner);
        }

        // TODO: Remove this
        public TileEntity Tile => Get<MapReferenceComponent>().Tile;

        public IEntityLogic EntityLogic => Game.Logic.EntityLogic(this);

        public abstract EntityType EntityType { get; }

        public T Get<T>() where T : IComponent => Components.Get<T>();
    }
}
