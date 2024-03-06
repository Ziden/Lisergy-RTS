
using Game;
using Game.Systems.Player;

namespace ClientSDK.Data
{
    /// <summary>
    /// Represents another player thats not the local player
    /// </summary>
    public class OtherPlayer : PlayerEntity
    {
        public OtherPlayer(PlayerProfile profile, IGame game) : base(profile, game)
        {
        }
    }
}
