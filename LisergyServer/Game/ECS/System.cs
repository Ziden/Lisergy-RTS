using Game.Events;
using Game.Events.Bus;
using System.Collections.Generic;

namespace Game.ECS
{

    public interface IGameSystem : IEventListener
    {
        void OnEnabled();

        void OnDisabled();
    }

    public abstract class GameSystem<ComponentType, EntityType> : IGameSystem where ComponentType : IComponent where EntityType : IEntity
    {
        public virtual void OnComponentAdded(EntityType owner, ComponentType component, EntitySharedEventBus<EntityType> events) { }
        public void OnDisabled() { }
        public void OnEnabled() { }
        public virtual void OnComponentRemoved(EntityType owner, ComponentType component, EntitySharedEventBus<EntityType> events) { }

    }
}
