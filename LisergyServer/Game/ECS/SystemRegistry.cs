using System;
using System.Collections.Generic;
using System.Reflection;

namespace Game.ECS
{

    /// <summary>
    /// Wrapper to be able to call without knowing generic types.
    /// Workaround so we can add/remove components by type and not generic type, should be removed 
    /// </summary>
    internal static class UntypedSystemRegistry
    {
        internal static Dictionary<Type, Delegate> componentAdder = new Dictionary<Type, Delegate>();
    
        internal static void OnAddComponent<EntityType>(EntityType owner, Type componentType, EntitySharedEventBus<EntityType> events) where EntityType : IEntity
        {
            if (componentAdder.TryGetValue(componentType, out var s)) s.DynamicInvoke(owner, owner.Components.Get(componentType), events);
        }

        internal static void OnRemovedComponent<EntityType>(EntityType owner, Type componentType, EntitySharedEventBus<EntityType> events) where EntityType : IEntity
        {
            if (componentAdder.TryGetValue(componentType, out var s)) s.DynamicInvoke(owner, owner.Components.Get(componentType), events);
        }
    }


    internal static class SystemRegistry<ComponentType, EntityType> where ComponentType : IComponent where EntityType : IEntity
    {

        public delegate void OnComponentAdd(EntityType e, ComponentType c, EntitySharedEventBus<EntityType> bus);


        private static Dictionary<Type, GameSystem<ComponentType, EntityType>> _systems = new Dictionary<Type, GameSystem<ComponentType, EntityType>>();

        public static void AddSystem<GameSystem>(GameSystem system) where GameSystem : GameSystem<ComponentType, EntityType>
        {
            _systems[typeof(ComponentType)] = system;
            UntypedSystemRegistry.componentAdder[typeof(ComponentType)] = new OnComponentAdd(system.OnComponentAdded);
        }

        internal static void OnAddComponent(EntityType owner, EntitySharedEventBus<EntityType> events)
        {
            if (_systems.TryGetValue(typeof(ComponentType), out var s))
            {
                s.OnComponentAdded(owner, owner.Components.Get<ComponentType>(), events);
            }
            /*
            if(SystemStore.componentAdder.TryGetValue(typeof(ComponentType), out var s)) {
                s.DynamicInvoke(owner, owner.Components.Get<ComponentType>(),  events);
            }
            */
        }

        internal static void OnRemovedComponent(EntityType owner, EntitySharedEventBus<EntityType> events)
        {
            if (_systems.TryGetValue(typeof(ComponentType), out var s))
            {
                s.OnComponentRemoved(owner, owner.Components.Get<ComponentType>(), events);
            }
            /*
            if (SystemStore.componentAdder.TryGetValue(typeof(ComponentType), out var s))
            {
                s.DynamicInvoke(owner, owner.Components.Get<ComponentType>(), events);
            }
            */
        }
    }
}
