
using Game.ECS;
using Game.Entity;
using Game.Events.Bus;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code.Views
{
    public interface IEntityView : IEventListener, IGameObject, IComponent
    {
        bool Instantiated { get; }
        void Instantiate();
        public abstract IEntity Entity { get; }
    }

    public abstract class EntityView<EntityType> : IEntityView where EntityType : IEntity
    {
        public virtual void OnUpdate(EntityType entity, List<IComponent> syncedComponents) { }
        public abstract EntityType Entity { get; }
        public abstract bool Instantiated { get; }
        public abstract GameObject GameObject { get; set; }
        IEntity IEntityView.Entity => Entity;
        public abstract void Instantiate();
    }
}
