namespace Game.Engine.ECLS
{

    /// <inheritdoc/>
    public class BaseEntityLogic<ComponentType> where ComponentType : IComponent
    {
        protected IGame Game => CurrentEntity.Game;
        public IEntity CurrentEntity { get; set; } // TODO: Think about this.
        public ComponentType GetComponent() => CurrentEntity.Get<ComponentType>();
    }
}
