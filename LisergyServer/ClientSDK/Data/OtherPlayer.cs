
using Game;
using Game.DataTypes;
using Game.Systems.Player;

namespace ClientSDK.Data
{
    /// <summary>
    /// Represents another player thats not the local player
    /// </summary>
    public class OtherPlayer : PlayerEntity
    {
        public OtherPlayer(GameId id, IGame game) : base(id, game)
        {
        }
    }
}
