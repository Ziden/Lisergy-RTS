using Game.Engine;
using Game.Engine.ECLS;
using System;
using System.Collections.Generic;

namespace ClientSDK.Sync
{
    public interface IComponentSync
    {
        void ProccessUpdate(IEntity entity, IComponent[] updated, uint[] removed);
        void OnComponentModified<ComponentType>(Action<IEntity, ComponentType, ComponentType> OnSync) where ComponentType : IComponent;
        void OnUpdate<ComponentType>(Action<IEntity, ComponentType, ComponentType> OnSync) where ComponentType : IComponent;
        void OnComponentRemoved<ComponentType>(Action<IEntity, ComponentType> OnRemoved) where ComponentType : IComponent;
        void OnComponentAdded<ComponentType>(Action<IEntity, ComponentType> OnAdded) where ComponentType : IComponent;
        void RemoveListener(object listener);
    }

    public class ComponentSynchronizer : IComponentSync
    {
        private Dictionary<Type, List<Action<IEntity, IComponent>>> _componentRemovals = new Dictionary<Type, List<Action<IEntity, IComponent>>>();
        private Dictionary<Type, List<Action<IEntity, IComponent>>> _componentAdded = new Dictionary<Type, List<Action<IEntity, IComponent>>>();
        private Dictionary<Type, List<Action<IEntity, IComponent, IComponent>>> _componentSyncs = new Dictionary<Type, List<Action<IEntity, IComponent, IComponent>>>();
        private Dictionary<Type, List<Type>> _listeners = new Dictionary<Type, List<Type>>();

        private List<(IComponent, IComponent)> _toSync = new List<(IComponent, IComponent)>();
        private List<IComponent> _added = new List<IComponent>();

        private IGameClient _client;

        public ComponentSynchronizer(IGameClient client)
        {
            _client = client;
        }

        public void Register() { }

        public void RemoveListener(object listener)
        {
            if (!_listeners.TryGetValue(listener.GetType(), out var listeners)) return;

            foreach (var t in listeners)
            {
                if (_componentRemovals.TryGetValue(t, out var removals))
                {
                    removals.RemoveAll(r => r.Target == listener);
                }
                if (_componentSyncs.TryGetValue(t, out var syncs))
                {
                    syncs.RemoveAll(r => r.Target == listener);
                }
            }
        }

        public void OnComponentRemoved<ComponentType>(Action<IEntity, ComponentType> OnSync) where ComponentType : IComponent
        {
            var t = typeof(ComponentType);
            if (!_componentRemovals.TryGetValue(t, out var syncList))
            {
                syncList = new List<Action<IEntity, IComponent>>();
                _componentRemovals[t] = syncList;
            }
            if (!_listeners.TryGetValue(OnSync.Target.GetType(), out var listeners))
            {
                listeners = new List<Type>();
                _listeners[OnSync.Target.GetType()] = listeners;
            }
            listeners.Add(t);
            syncList.Add((entity, component) => OnSync(entity, (ComponentType)component));
        }

        public void OnUpdate<ComponentType>(Action<IEntity, ComponentType, ComponentType> OnSync) where ComponentType : IComponent
        {
            OnComponentAdded<ComponentType>((e, c) => OnSync(e, default!, c));
            OnComponentModified(OnSync);
            OnComponentAdded<ComponentType>((e, c) => OnSync(e, c, default!));
        }

        public void OnComponentAdded<ComponentType>(Action<IEntity, ComponentType> OnSync) where ComponentType : IComponent
        {
            var t = typeof(ComponentType);
            if (!_componentAdded.TryGetValue(t, out var syncList))
            {
                syncList = new List<Action<IEntity, IComponent>>();
                _componentAdded[t] = syncList;
            }
            if (!_listeners.TryGetValue(OnSync.Target.GetType(), out var listeners))
            {
                listeners = new List<Type>();
                _listeners[OnSync.Target.GetType()] = listeners;
            }
            listeners.Add(t);
            syncList.Add((entity, component) => OnSync(entity, (ComponentType)component));
        }

        public void OnComponentModified<ComponentType>(Action<IEntity, ComponentType, ComponentType> OnSync) where ComponentType : IComponent
        {
            var t = typeof(ComponentType);
            if (!_componentSyncs.TryGetValue(t, out var syncList))
            {
                syncList = new List<Action<IEntity, IComponent, IComponent>>();
                _componentSyncs[t] = syncList;
            }
            if (!_listeners.TryGetValue(OnSync.Target.GetType(), out var listeners))
            {
                listeners = new List<Type>();
                _listeners[OnSync.Target.GetType()] = listeners;
            }
            listeners.Add(t);
            syncList.Add((entity, oldComponent, newComponent) => OnSync(entity, (ComponentType)oldComponent, (ComponentType)newComponent));
        }

        public void ProccessUpdate(IEntity currentEntity, IComponent[] updated, uint[] removed)
        {
            var view = _client.Modules.Views.GetEntityView(currentEntity);

            _toSync.Clear();
            _added.Clear();

            foreach (var newComponent in updated)
            {
                if (_componentSyncs.ContainsKey(newComponent.GetType()))
                {
                    if (currentEntity.Components.GetComponents().TryGetValue(newComponent.GetType(), out var existing))
                    {
                        _toSync.Add((existing, newComponent));
                    }
                    else
                    {
                        _added.Add(newComponent);
                    }
                }
                currentEntity.Components.Save(newComponent);
            }

            foreach (var removedId in removed)
            {
                var componentType = Serialization.GetType(removedId);
                var comp = currentEntity.Components.GetByType(componentType);
                currentEntity.Components.GetComponents().Remove(componentType);
                if (_componentRemovals.TryGetValue(componentType, out var removals))
                {
                    foreach (var removal in removals)
                    {
                        view.RunWhenRendered(() => removal(currentEntity, comp));
                    }
                }
            }

            foreach (var toSync in _toSync)
            {
                foreach (var sync in _componentSyncs[toSync.Item2.GetType()])
                {
                    view.RunWhenRendered(() => sync(currentEntity, toSync.Item1, toSync.Item2));
                }
            }

            foreach (var toSync in _added)
            {
                foreach (var add in _componentAdded[toSync.GetType()])
                {
                    view.RunWhenRendered(() => add(currentEntity, toSync));
                }
            }
        }
    }
}
