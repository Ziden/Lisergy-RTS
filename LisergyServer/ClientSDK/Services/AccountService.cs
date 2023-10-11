using Game.Network.ClientPackets;
using Game;
using Game.Events.ServerEvents;
using System;
using Game.DataTypes;

namespace ClientSDK.Services
{
    /// <summary>
    /// Service responsible for handling authentication and specific account and profile information
    /// </summary>
    public interface IAccountService : IClientService
    {
        event Action<GameId>? OnAuthenticated;
        void SendAuthenticationPacket(string username, string password);
    }

    public class AccountService : IAccountService
    {
        public event Action<GameId>? OnAuthenticated;

        private IGameNetwork _network;

        public AccountService(GameClient client)
        {
            _network = client.ClientNetwork;
        }

        public void Register()
        {
            _network.On<AuthResultPacket>(OnAuthResult);
        }

        public void SendAuthenticationPacket(string username, string password)
        {
            _network.SendToServer(new AuthPacket()
            {
                Login = username,
                Password = password
            });
        }

        private void OnAuthResult(AuthResultPacket packet)
        {
            if(packet.Success)
            {
                OnAuthenticated?.Invoke(packet.PlayerID);
            }
        }
    }
}
