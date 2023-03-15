using Game.Events;
using Game.Events.Bus;
using System;

namespace Game.ECS
{
    internal class BusContext<SystemType, EntityType, ComponentType> where SystemType : IGameSystem where EntityType : IEntity where ComponentType : IComponent
    {
        EntityType Entity;
        ComponentType Component;
        SystemType System;
    }

    public class ComponentEventBus<EntityType> : IEventListener where EntityType : IEntity
    {
        private EventBus<BaseEvent> _bus = new EventBus<BaseEvent>();

        public void RegisterComponentEvent<EventType, ComponentType>(IGameSystem system, EntityType entity, ComponentType component, Action<EntityType, ComponentType, EventType> callback) where EventType : BaseEvent where ComponentType : IComponent
        {
            _bus.Register<EventType>(system, ev => callback(entity, component, ev));
        }



        public void RegisterEvent<EventType>(EventType ev)
        {

        }

        public void Call<EventType>(EntityType entity, EventType ev) where EventType : BaseEvent
        {
            _bus.Call(ev);
        }
    }
}
