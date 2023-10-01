using Game.Events;
using Game.Events.Bus;
using System;
using System.Diagnostics;

namespace Game.ECS
{

    public interface IGameSystem : IEventListener
    {
    }

    public abstract class GameSystemBase : IGameSystem
    {
        public Type EntityType;
        public Type ComponentType;
    }

    public abstract class GameSystem<ComponentType, EntityType> : GameSystemBase where ComponentType : IComponent where EntityType : IEntity
    {
        public SystemEventBus<EntityType, ComponentType> SystemEvents = new SystemEventBus<EntityType, ComponentType>();

        internal virtual void OnComponentAdded(EntityType owner, ComponentType component) { }
        public virtual void OnDisabled() { }
        public virtual void OnEnabled() { }
        internal virtual void OnComponentRemoved(EntityType owner, ComponentType component) { }

    }
}
