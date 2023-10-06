using Game.DataTypes;
using Game.Network;

namespace Game.ECS
{

    public interface IEntity : IDeltaUpdateable, IDeltaTrackable
    {
        public IComponentSet Components { get; }

        public T Get<T>() where T : IComponent;

        public GameId EntityId { get; }

        /// <summary>
        /// Gets the player owner of the entity.
        /// Will have zero value for entities owned by server (NPCS)
        /// </summary>
        public GameId OwnerID { get; }

        public IGame Game { get; }

        public IEntityLogic EntityLogic { get; }

    }


}
