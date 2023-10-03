using Game.Systems.Battler;

namespace Game.ECS
{
    public interface IComponentLogic<T> where T : IComponent
    {
        public IEntity Entity { get; set; }

        public T Component { get; }
    }

    public class BaseComponentLogic<ComponentType> : IComponentLogic<ComponentType> where ComponentType : IComponent
    {
        public IEntity Entity { get; set; }

        public ComponentType Component => Entity.Get<ComponentType>();
    }
}
