
using Game;
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
    }
}
