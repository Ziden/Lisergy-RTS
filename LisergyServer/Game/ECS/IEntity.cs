using Game.DataTypes;
using Game.Network;

namespace Game.ECS
{

    public interface IEntity : IOwnable, IDeltaUpdateable
    {
        public IComponentSet Components { get; }

        public T Get<T>() where T : IComponent;

        public GameId EntityId { get; }
    }


}
