using Game.Systems.Battler;

namespace Game.ECS
{
    /// <summary>
    /// Base entity logic interface.
    /// Entity logic is the main responsible for modifying entity components.
    /// </summary>
    public interface IComponentLogic<T> where T : unmanaged, IComponent 
    {
        public IEntity Entity { get; set; }

        public ref T GetComponent();
    }

    /// <inheritdoc/>
    public class BaseEntityLogic<ComponentType> : IComponentLogic<ComponentType> where ComponentType : unmanaged, IComponent
    {
        protected IGame Game => Entity.Game;
        public IEntity Entity { get; set; }
        public ref ComponentType GetComponent() => ref Entity.Get<ComponentType>();
    }
}
