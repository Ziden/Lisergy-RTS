namespace Game.Engine.ECLS
{
    /// <inheritdoc />
    public class BaseEntityLogic<ComponentType>
	{
		protected IGame Game => CurrentEntity.Game;
		public IEntity CurrentEntity { get; set; } // TODO: Think about this.

		public ComponentType GetComponent()
		{
			return CurrentEntity.Get<ComponentType>();
		}
	}
}