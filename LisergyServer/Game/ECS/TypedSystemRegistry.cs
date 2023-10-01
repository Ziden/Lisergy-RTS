using Game.Events;
using System;
using System.Collections.Generic;

namespace Game.ECS
{

    static class TypedSystemRegistry<ComponentType, EntityType> where ComponentType : IComponent where EntityType : IEntity
    {
        static readonly Dictionary<Type, GameSystem<ComponentType, EntityType>> _systemsPerComponent = new Dictionary<Type, GameSystem<ComponentType, EntityType>>();

        public static void AddSystem<GameSystem>(GameSystem system) where GameSystem : GameSystem<ComponentType, EntityType>
        {
            _systemsPerComponent[typeof(ComponentType)] = system;
            system.EntityType = typeof(EntityType);
            system.ComponentType = typeof(ComponentType);
            system.OnEnabled();
        }

        internal static void OnAddComponent(EntityType owner)
        {
            if (_systemsPerComponent.TryGetValue(typeof(ComponentType), out GameSystem<ComponentType, EntityType> s))
            {
                s.OnComponentAdded(owner, owner.Components.Get<ComponentType>());
            }
        }

        internal static void OnRemovedComponent(EntityType owner)
        {
            if (_systemsPerComponent.TryGetValue(typeof(ComponentType), out GameSystem<ComponentType, EntityType> s))
            {
                s.OnComponentRemoved(owner, owner.Components.Get<ComponentType>());
            }
        }
    }
}
