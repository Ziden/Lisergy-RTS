using ClientSDK.Data;
using ClientSDK.SDKEvents;
using Game;
using Game.Engine;
using Game.Engine.DataTypes;
using Game.Entities;
using Game.Events.ServerEvents;
using Game.Network.ClientPackets;
using Game.Systems.Map;
using Game.Systems.Player;

namespace ClientSDK.Services
{
    /// <summary>
    /// Service responsible for handling authentication and specific account and profile information.
    /// Will perform the initial login flow until world is joined.
    /// </summary>
    public interface IAccountModule : IClientModule
    {
        /// <summary>
        /// Sends a request to authenticate to server
        /// </summary>
        void SendAuthenticationPacket(string username, string password);
    }

    public class AccountModule : IAccountModule
    {
        private PlayerProfileComponent _profile;
        private GameClient _client;

        public AccountModule(GameClient client)
        {
            _client = client;
        }

        public void Register()
        {
            _client.Network.OnInput<LoginResultPacket>(OnAuthResult);
            _client.Network.OnInput<GameSpecPacket>(OnReceiveGameSpec);
        }

        public void SendAuthenticationPacket(string username, string password)
        {
            _client.Network.SendToServer(new LoginPacket()
            {
                Login = username,
                Password = password
            }, ServerType.ACCOUNT);
        }

        private void OnAuthResult(LoginResultPacket packet)
        {
            if (packet.Success)
            {
                _profile = packet.Profile;
                ((ClientNetwork)_client.Network).SetCredentials(packet);
            }
        }

        private void OnReceiveGameSpec(GameSpecPacket ev)
        {
            _client.SDKLog.Debug("Initialized Specs");
            var game = new LisergyGame(ev.Spec, new GameLog("[Client Game]"), _client.Network, isClientGame: true);
            var world = new ClientWorld(game, (ushort)ev.MapSizeX, (ushort)ev.MapSizeY);
            game.SetupWorld(world);
            _client.InitializeGame(game);
            var e = _client.Game.Entities[_profile.PlayerId];
            if (e == null)
            {
                e = _client.Game.Entities.CreateEntity(EntityType.Player, GameId.ZERO, _profile.PlayerId);
            }
            var player = new PlayerModel(game, e);
            e.Save(_profile);
            _client.ClientEvents.Call(new GameStartedEvent(game, player));
            _client.Network.SendToServer(new JoinWorldMapCommand());
        }
    }
}
