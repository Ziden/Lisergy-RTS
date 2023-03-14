using Game.Entity;
using Game.Events;
using Game.Events.Bus;
using System;
using System.Collections.Generic;
using System.Text;

namespace Game
{

    public class ComponentStore<ComponentType, ComponentOwner> where ComponentType : class, IComponent<ComponentType, ComponentOwner>
    {
        private Dictionary<Type, ComponentType> _components = new Dictionary<Type, ComponentType> ();

        private ComponentOwner _owner;

        public EventBus Events = new EventBus();

        public ComponentStore(ComponentOwner owner)
        {
            _owner = owner;
        }

        public IReadOnlyCollection<ComponentType> GetComponents()
        {
            return _components.Values;
        }

        public T GetComponent<T>() where T: class, ComponentType
        {
            if(_components.TryGetValue(typeof(T), out var component))
            {
                return component as T;
            }
            return default;
        }

        public void AddComponent<T>() where T: ComponentType
        {
            var component = Activator.CreateInstance<T>();
            component.Owner = _owner;
            component.Events = Events;
            _components[typeof(T)] = component;
            component.OnAdded();
        }
    }

    public interface IComponent<ComponentType, OwnerType> where ComponentType : class, IComponent<ComponentType, OwnerType>
    {
        public OwnerType Owner { get; set; }

        public EventBus Events { get; set; }

        void OnAdded();

        void OnRemoved();
    }

    public abstract class TileComponent : IComponent<TileComponent, Tile>, IEventListener
    {
        public Tile Owner { get; set; }

        Tile IComponent<TileComponent, Tile>.Owner { get => Owner; set => Owner = value; }
     
        public EventBus Events { get; set; }

        public virtual void OnAdded()
        {

        }

        public virtual void OnRemoved()
        {
            Events.Clear(this);
        }
    }

   

    public interface IComponentHolder<ComponentType, ComponentOwner> where ComponentType : class, IComponent<ComponentType, ComponentOwner> {
        public T GetComponent<T>() where T: ComponentType;

        public void AddComponent<T>() where T : ComponentType;

        public IReadOnlyCollection<ComponentType> GetComponents();

        public void CallEvent(GameEvent ev);
    }

}
