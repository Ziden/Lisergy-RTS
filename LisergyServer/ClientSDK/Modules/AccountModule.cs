using Game.Network.ClientPackets;
using Game;
using Game.Events.ServerEvents;
using System;
using Game.DataTypes;
using Telepathy;
using ClientSDK.Data;
using Game.Systems.Player;

namespace ClientSDK.Services
{
    /// <summary>
    /// Service responsible for handling authentication and specific account and profile information.
    /// Will perform the initial login flow until world is joined.
    /// </summary>
    public interface IAccountModule : IClientModule
    {
        event Action<GameId> OnAuthenticated;
        event Action<IGame> OnSpecsReceived;
        void SendAuthenticationPacket(string username, string password);

        /// <summary>
        /// Gets the local player
        /// </summary>
        PlayerEntity LocalPlayer { get; }
    }

    public class AccountModule : IAccountModule
    {
        public event Action<GameId> OnAuthenticated;
        public event Action<IGame> OnSpecsReceived;

        public PlayerEntity LocalPlayer { get; private set; }
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
                OnAuthenticated?.Invoke(packet.PlayerID);
            }
        }

        private void OnReceiveGameSpec(GameSpecPacket ev)
        {
            Log.Debug("Initialized Specs");
            var game = new LisergyGame(ev.Spec);
            var world = new ClientWorld(int.MaxValue, (ushort)ev.MapSizeX, (ushort)ev.MapSizeY);
            game.SetupGame(world, _client.Network);
            _client.InitializeGame(game);
            OnSpecsReceived?.Invoke(game);
            LocalPlayer = new PlayerEntity(_playerId, game);
            _client.Network.SendToServer(new JoinWorldPacket());
        }
    }
}
