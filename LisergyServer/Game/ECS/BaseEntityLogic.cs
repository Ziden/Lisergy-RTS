using Game.Systems.Battler;

namespace Game.ECS
{
    public interface IComponentLogic<T> where T : IComponent
    {
        public IEntity Entity { get; set; }

        public T GetComponent();
    }

    public class BaseEntityLogic<ComponentType> : IComponentLogic<ComponentType> where ComponentType : IComponent
    {
        protected IGame Game => Entity.Game;
        public IEntity Entity { get; set; }
        public ComponentType GetComponent() => Entity.Get<ComponentType>();
    }
}
