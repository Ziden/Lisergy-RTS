
using Game;
using Game.Engine.ECLS;
using Game.Systems.Player;

namespace ClientSDK.Data
{
    /// <summary>
    /// Represents another player thats not the local player
    /// </summary>
    public class OtherPlayer(IGame game, IEntity e) : PlayerModel(game, e);
}
