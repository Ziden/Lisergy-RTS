using Game.DataTypes;
using Game.Events;
using Game.Systems.Party;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Game.ECS
{
    public static class UntypedRegistry
    {
        internal static readonly Dictionary<Type, IGameSystem> _systemsPerComponent = new Dictionary<Type, IGameSystem>();

        public static void RunEventsForComponent<EventType>(Type componentType, IEntity entity, EventType ev) where EventType : BaseEvent
        {
            if (_systemsPerComponent.TryGetValue(componentType, out var system))
            {
                system.CallEvent(entity, ev);
            }
        }
    }

    static class ComponentSystemRegistry<ComponentType> where ComponentType : IComponent
    {
        static readonly Dictionary<Type, GameSystem<ComponentType>> _perComponent = new Dictionary<Type, GameSystem<ComponentType>>();

        public static void AddSystem<GameSystem>(GameSystem system) where GameSystem : GameSystem<ComponentType>
        {
            _perComponent.Add(typeof(ComponentType), system);
            UntypedRegistry._systemsPerComponent.Add(typeof(ComponentType), system);
            system.OnEnabled();
        }

        internal static void OnAddComponent(IEntity owner)
        {
            if (!GameSystems.IsLoaded) throw new Exception("Systems not loaded");
            if (_perComponent.TryGetValue(typeof(ComponentType), out GameSystem<ComponentType> s))
            {
                s.OnComponentAdded(owner, owner.Components.Get<ComponentType>());
            }
        }

        internal static void OnRemovedComponent(IEntity owner)
        {
            if (_perComponent.TryGetValue(typeof(ComponentType), out GameSystem<ComponentType> s))
            {
                s.OnComponentRemoved(owner, owner.Components.Get<ComponentType>());
            }
        }
    }
}
