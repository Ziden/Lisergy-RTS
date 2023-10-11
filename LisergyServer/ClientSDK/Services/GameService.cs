using Game;
using Game.DataTypes;
using Game.Events.ServerEvents;

namespace ClientSDK.Services
{

    public interface IGameService : IClientService { }

    public class GameService : IPlayerService
    {
        private IGameNetwork _network;

        public GameService(IGameNetwork network)
        {
            _network = network;
        }

        public void Register()
        {
            _network.On<GameSpecPacket>(OnReceiveGameSpec);
        }

        private void OnReceiveGameSpec(GameSpecPacket ev)
        {
            var game = new LisergyGame(ev.Spec);

        }
    }
}
