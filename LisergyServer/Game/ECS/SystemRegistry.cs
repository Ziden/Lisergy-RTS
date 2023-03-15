using System;
using System.Collections.Generic;
using System.Reflection;

namespace Game.ECS
{
    internal static class SystemRegistry<ComponentType, EntityType> where ComponentType : IComponent where EntityType : IEntity
    {

        private static Dictionary<Type, GameSystem<ComponentType, EntityType>> _systems = new Dictionary<Type, GameSystem<ComponentType, EntityType>>();

        public static void AddSystem<GameSystem>(GameSystem system) where GameSystem : GameSystem<ComponentType, EntityType>
        {
            _systems[typeof(ComponentType)] = system;
        }

        internal static void OnAddComponent(EntityType owner, ComponentType component, ComponentEventBus<EntityType> events)
        { 
            if(_systems.TryGetValue(component.GetType(), out var s)) {
                s.OnAdded(owner, component, events);
            }
        }

        internal static void OnRemovedComponent(EntityType owner, ComponentType component, ComponentEventBus<EntityType> events)
        {
            if (_systems.TryGetValue(component.GetType(), out var s))
            {
                s.OnRemoved(owner, component, events);
            }
        }
    }
}
