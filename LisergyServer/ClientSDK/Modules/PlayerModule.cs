using Game;
using Game.DataTypes;

namespace ClientSDK.Services
{

    public interface IPlayerModule : IClientModule
    {
    }

    public class PlayerModule : IPlayerModule
    {
        private IGameClient _client;

        public PlayerModule(IGameClient client)
        {
            _client = client;
        }

        public void Register()
        {
            _client.Modules.Account.OnAuthenticated += OnAuthenticated;
        }

        private void OnAuthenticated(GameId id)
        {
            Log.Debug("Logged in as Player " + id);
        }
    }
}
