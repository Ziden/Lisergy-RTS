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
        public abstract void OnAdded(EntityType owner, ComponentType component, ComponentEventBus<EntityType> events);
        public void OnDisabled() { }
        public void OnEnabled() { }
        public abstract void OnRemoved(EntityType owner, ComponentType component, ComponentEventBus<EntityType> events);

    }
}
