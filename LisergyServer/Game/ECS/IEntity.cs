using Game.DataTypes;
using Game.Player;

namespace Game.ECS
{

    public interface IEntity : IOwnable
    {
        public IComponentSet Components { get; }

        public GameId EntityId { get; }

        //public PlayerEntity Owner { get; }
    }


}
