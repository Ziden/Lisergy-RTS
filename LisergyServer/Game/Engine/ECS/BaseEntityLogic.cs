namespace Game.Engine.ECLS
{

    /// <inheritdoc/>
    public class BaseEntityLogic<ComponentType> where ComponentType : IComponent
    {
        protected IGame Game => Entity.Game;
        public IEntity Entity { get; set; }
        public ComponentType GetComponent() => Entity.Get<ComponentType>();
    }
}
