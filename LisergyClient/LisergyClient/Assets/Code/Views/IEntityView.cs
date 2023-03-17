
using Game.ECS;
using Game.Events;
using Game.Events.Bus;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code.Views
{
    public interface IEntityView : IEventListener, IGameObject, IComponent
    {
        bool Instantiated { get; }

        void Instantiate();
    }

    public abstract class EntityView<EntityType> : IEntityView where EntityType : IEntity
    {
        public abstract EntityType Entity { get; }

        public abstract bool Instantiated { get; }
        public abstract GameObject GameObject { get; set; }

        public abstract void Instantiate();

        private ViewEvents<EntityType> _viewEvents;

        public void RegisterViewEvent<EventType, ViewType>(Action<EntityType, ViewType, EventType> cb) where EventType : GameEvent where ViewType : IEntityView
        {
            if (_viewEvents == null)
            {
                _viewEvents = new ViewEvents<EntityType>(Entity);
            }
            _viewEvents.RegisterEvent(cb);
        }
    }

    public class ViewEventBus<EntityType> : EntitySharedEventBus<EntityType> where EntityType : IEntity
    {
        public override ComponentType GetComponent<ComponentType>(IEntity e) 
        {
            return (ComponentType)GameView.GetView(e);
        }
    }

    public class ViewEvents<EntityType> : IEventListener where EntityType : IEntity
    {
        internal EntityType _owner;

        private HashSet<string> _registered = new HashSet<string>();

        internal static Dictionary<Type, ViewEventBus<EntityType>> _buses = new Dictionary<Type, ViewEventBus<EntityType>>();

        public EntitySharedEventBus<EntityType> GetEventBus()
        {
            if (!_buses.TryGetValue(typeof(EntityType), out var bus))
            {
                bus = new ViewEventBus<EntityType>();
                _buses[typeof(EntityType)] = bus;
            }
            return bus;
        }

        public ViewEvents(EntityType owner)
        {
            _owner = owner;
        }

        public void CallEvent(BaseEvent ev)
        {
            GetEventBus().Call(_owner, ev);
        }

        /// <summary>
        /// Adds global event hook to convert to entity events.
        /// Only add one per Event/View combination
        /// </summary>  
        public void RegisterEvent<EventType, ViewType>(Action<EntityType, ViewType, EventType> cb) where EventType : GameEvent where ViewType : IEntityView
        {
            var key = $"{typeof(EventType)}{typeof(ViewType)}";
            if(!_registered.Contains(key)) {
                _registered.Add(key);
                GameView.Events.Register<EventType>(this, e => GetEventBus().Call(_owner, e));
            }
            GetEventBus().RegisterComponentEvent(cb);
        }
    }
}
