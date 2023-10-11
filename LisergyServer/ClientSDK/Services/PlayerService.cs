using Game;
using Game.DataTypes;

namespace ClientSDK.Services
{

    public interface IPlayerService : IClientService
    {
    }

    public class PlayerService : IPlayerService
    {
        private IGameClient _client;

        public PlayerService(IGameClient client)
        {
            _client = client;
        }

        public void Register()
        {
            _client.Services.Account.OnAuthenticated += OnAuthenticated;
        }

        private void OnAuthenticated(GameId id)
        {
            Log.Debug("Logged in as Player " + id);
        }
    }
}
