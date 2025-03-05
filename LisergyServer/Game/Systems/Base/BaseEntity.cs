
using Game.Engine.DataTypes;
using Game.Engine.ECLS;

namespace Game.Entities
{
    public class BaseEntity : IEntity
    {
        internal GameId _entityId;
        public ComponentSet Components { get; private set; }
        public EntityType EntityType { get; }
        public ref readonly GameId EntityId => ref _entityId;
        public IGame Game { get; private set; }
        public BaseEntity(GameId id, IGame game, EntityType type)
        {
            Game = game;
            _entityId = id;
            EntityType = type;
            Components = new ComponentSet(this);
        }

        public GameId OwnerID => Game.Entities.GetParent(EntityId)?.EntityId ?? GameId.ZERO;
        public EntityLogic Logic => Game.Logic.GetEntityLogic(this);
        public T Get<T>() where T : IComponent => Components.Get<T>();
        public void Save<T>(in T component) where T : IComponent => Components.Save(component);
        public override string ToString()
        {
            return $"<Entity {EntityType} {EntityId} Ct={Components.GetComponents().Count}>";
        }
    }
}
