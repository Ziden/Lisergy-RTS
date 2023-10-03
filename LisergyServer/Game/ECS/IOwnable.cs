using Game.DataTypes;
using Game.Systems.Player;

namespace Game.ECS
{

    public interface IOwnable
    {
        PlayerEntity? Owner { get; }

        GameId OwnerID { get; }
    }
}
