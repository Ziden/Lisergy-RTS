using Game.Systems.Battler;

namespace Game.ECS
{
    public interface IComponentLogic<T> where T : IComponent
    {
        public IEntity Entity { get; set; }

        public T Component { get; }
    }

    public class BaseEntityLogic<ComponentType> : IComponentLogic<ComponentType> where ComponentType : IComponent
    {
        protected IGame Game => Entity.Game;
        public IEntity Entity { get; set; }
        public ComponentType Component => Entity.Get<ComponentType>();

        public void FinishLogic() { }
    }
}
