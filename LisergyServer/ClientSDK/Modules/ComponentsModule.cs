using ClientSDK.SDKEvents;
using Game.ECS;
using Game.Events;
using Game;
using System;
using System.Collections.Generic;

namespace ClientSDK.Modules
{
    /// <summary>
    /// Enables the game to more specifically control how to update components.
    /// Components can be synced automatically (just copy data) or have manual syncs to handle things manually.
    /// </summary>
    public interface IComponentsModule : IClientModule
    {
        /// <summary>
        /// Registers a component sync. 
        /// Whenever the given entity type has the given component type updated, instead of the values simply being copied
        /// the sync code will be called
        /// </summary>
        void OnComponentUpdate<EntityType, ComponentType>(Action<IEntity, ComponentType> OnSync) where ComponentType : IComponent where EntityType : IEntity;

        /// <summary>
        /// Updates the components of the given entity
        /// </summary>
        void UpdateComponents(IEntity currentEntity, IComponent[] newComponents);
    }

    public class ComponentsModule : IComponentsModule
    {
        private List<IComponent> _toSync = new List<IComponent>();
        private Dictionary<string, Delegate> _componentSyncs = new Dictionary<string, Delegate>();
        public ComponentsModule() 
        { 
        
        }
        public void Register()
        {
           
        }

        private string Key(Type entityType, Type componentType) => $"{entityType.Name}:{componentType.Name}";

        public void OnComponentUpdate<EntityType, ComponentType>(Action<IEntity, ComponentType> OnSync) where ComponentType : IComponent where EntityType : IEntity
        {
            _componentSyncs[Key(typeof(EntityType), typeof(ComponentType))] = OnSync;
            Log.Debug($"Registered component {typeof(ComponentType).Name} sync for {typeof(EntityType).Name}");
        }


        public void UpdateComponents(IEntity currentEntity, IComponent[] newComponents)
        {
            foreach (var newValue in newComponents)
            {
                if(_componentSyncs.TryGetValue(Key(currentEntity.GetType(), newValue.GetType()), out var sync))
                {
                    sync.DynamicInvoke(currentEntity, newValue);
                } 
                currentEntity.Components.Save(newValue);
            }
        }
    }
}
