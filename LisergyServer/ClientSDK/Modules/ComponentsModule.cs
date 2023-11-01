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
    /// This module is able to implement callbacks from when a given component is synced so the client can react to it.
    /// </summary>
    public interface IComponentsModule : IClientModule
    {
        /// <summary>
        /// Updates the components of the given entity
        /// </summary>
        void UpdateComponents(IEntity currentEntity, IComponent[] newComponents);

        /// <summary>
        /// Registers a component sync. 
        /// Whenever the given entity type has the given component type updated, instead of the values simply being copied
        /// the sync code will be called.
        /// The callback has the Entity, OLD VALUE and NEW VALUE parameters.
        /// </summary>
        void OnComponentUpdate<ComponentType>(Action<IEntity, ComponentType, ComponentType> OnSync) where ComponentType : IComponent;
    }

    public class ComponentsModule : IComponentsModule
    {
        private Dictionary<Type, List<Delegate>> _componentSyncs = new Dictionary<Type, List<Delegate>>();
        private List<(IComponent, IComponent)> _toSync = new List<(IComponent, IComponent)>();
        public void Register() {}

        public void OnComponentUpdate<ComponentType>(Action<IEntity, ComponentType, ComponentType> OnSync) where ComponentType : IComponent
        {
            var t = typeof(ComponentType);
            if (!_componentSyncs.TryGetValue(t, out var syncList))
            {
                syncList = new List<Delegate>();
                _componentSyncs[t] = syncList;
            }
            syncList.Add(OnSync);
        }

        /// <summary>
        /// Updates the components of the given entity.
        /// Will copy all new values to old values
        /// Any registered component sync callbacks will be called after all updates are done
        /// </summary>
        public void UpdateComponents(IEntity currentEntity, IComponent[] newComponents)
        {

            _toSync.Clear();
            foreach (var newComponent in newComponents)
            {
                if(_componentSyncs.ContainsKey(newComponent.GetType()))
                {
                    _toSync.Add((currentEntity.Components.GetByType(newComponent.GetType()), newComponent));
                } 
                currentEntity.Components.Save(newComponent);
            }
            foreach(var toSync in _toSync)
            {
                foreach(var deleg in _componentSyncs[toSync.Item1.GetType()])
                {
                    deleg.DynamicInvoke(currentEntity, toSync.Item1, toSync.Item2);
                }
            }
        }
    }
}
