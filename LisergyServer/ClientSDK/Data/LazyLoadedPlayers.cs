using Game.Systems.Player;
using Game;
using Game.Engine.DataTypes;
using Game.World;

namespace ClientSDK.Data
{
    /// <summary>
    /// Will create players as players are requests but not available
    /// </summary>
    public class LazyLoadedPlayers : WorldPlayers
    {
        private IGame _game;

        public LazyLoadedPlayers(IGame game) : base(int.MaxValue)
        {
            _game = game;
        }

        public override PlayerEntity GetPlayer(GameId id)
        {
            var p = base.GetPlayer(id);
            if (p == null) _players[id] = new OtherPlayer(new PlayerProfile(id), _game);
            return base.GetPlayer(id);
        }
    }
}
