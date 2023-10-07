using Game.DataTypes;
using Game.Network;

namespace Game.ECS
{

    public interface IEntity : IEntityDeltaTrackable
    {
        public EntityType EntityType { get; }
        public IComponentSet Components { get; }

        public T Get<T>() where T : IComponent;

        public ref readonly GameId EntityId { get; }
        public ref readonly GameId OwnerID { get; }

        public IGame Game { get; }

        public IEntityLogic EntityLogic { get; }

    }


}
