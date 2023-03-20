using Game.DataTypes;
using Game.Player;

namespace Game
{

    public interface IOwnable
    {
        PlayerEntity Owner { get; }

        GameId OwnerID { get; }
    }
}
