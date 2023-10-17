using Game.Network.ClientPackets;
using Game;
using Game.Events.ServerEvents;
using System;
using Game.DataTypes;
using ClientSDK.Data;
using Game.Systems.Player;
using ClientSDK.SDKEvents;

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
        private GameId _playerId;
        private GameClient _client;

        public AccountModule(GameClient client)
        {
            _client = client;
        }

        public void Register()
        {
            _client.Network.On<AuthResultPacket>(OnAuthResult);
            _client.Network.On<GameSpecPacket>(OnReceiveGameSpec);
        }

        public void SendAuthenticationPacket(string username, string password)
        {
            _client.Network.SendToServer(new AuthPacket()
            {
                Login = username,
                Password = password
            });
        }

        private void OnAuthResult(AuthResultPacket packet)
        {
            if (packet.Success)
            {
                _playerId = packet.PlayerID;
            }
        }

        private void OnReceiveGameSpec(GameSpecPacket ev)
        {
            Log.Debug("Initialized Specs");
            var game = new LisergyGame(ev.Spec);
            var world = new ClientWorld(game, int.MaxValue, (ushort)ev.MapSizeX, (ushort)ev.MapSizeY);
            game.SetupGame(world, _client.Network);
            _client.InitializeGame(game);
            _client.ClientEvents.Call(new GameStartedEvent(game, new PlayerEntity(_playerId, game)));
            _client.Network.SendToServer(new JoinWorldPacket());
        }
    }
}
