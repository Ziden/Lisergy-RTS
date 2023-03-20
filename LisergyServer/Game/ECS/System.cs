using Game.Events.Bus;

namespace Game.ECS
{

    public interface IGameSystem : IEventListener
    {
        void OnEnabled();

        void OnDisabled();
    }

    public abstract class GameSystem<ComponentType, EntityType> : IGameSystem where ComponentType : IComponent where EntityType : IEntity
    {
        internal virtual void OnComponentAdded(EntityType owner, ComponentType component, EntityEventBus events) { }
        public void OnDisabled() { }
        public void OnEnabled() { }
        internal virtual void OnComponentRemoved(EntityType owner, ComponentType component, EntityEventBus events) { }

    }
}
