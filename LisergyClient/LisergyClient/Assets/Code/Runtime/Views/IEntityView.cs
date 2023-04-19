
using Game.ECS;
using Game.Events.Bus;
using System.Collections.Generic;
using Game.Events.GameEvents;
using UnityEngine;
using System;

namespace Assets.Code.Views
{
    public enum EntityViewState
    {
        CLEAR, INSTANTIATING, INSTANTIATED
    } 

    public interface IEntityView : IEventListener, IGameObject, IComponent
    {
        bool NeedsInstantiate { get; }
        public abstract IEntity Entity { get; }
    }

    public abstract class EntityView<EntityType> : IEntityView where EntityType : IEntity
    {
        protected event Action<GameObject> OnInstantiated;

        private GameObject _obj;

        private EntityViewState _state;

        public virtual void OnUpdate(EntityType entity, List<IComponent> syncedComponents) { }
        public abstract EntityType Entity { get; }
        public bool NeedsInstantiate => _state == EntityViewState.CLEAR;

        public bool Instantiated => _state == EntityViewState.INSTANTIATED;

        public GameObject GameObject { get => _obj; }

        protected void SetGameObject(GameObject o)
        {
            _obj = o;
            OnInstantiated?.Invoke(_obj);
            OnInstantiated = null;
            _state = EntityViewState.INSTANTIATED;
        }

        IEntity IEntityView.Entity => Entity;
        protected abstract void InstantiationImplementation();

        public void Instantiate()
        {
            if (!NeedsInstantiate) return;
            _state = EntityViewState.INSTANTIATING;

            InstantiationImplementation();
        }

        public void OnInstantiate(Action<GameObject> callback)
        {
            if (_state == EntityViewState.INSTANTIATED)
                callback(GameObject);
            else
                OnInstantiated += callback;
        }
    }
}
