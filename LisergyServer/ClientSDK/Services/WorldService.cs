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
    public interface IWorldService : IClientService
    {
    }

    public class WorldService : IWorldService
    {

        private IGameClient _client;

        public WorldService(IGameClient client)
        {
            _client = client;
        }

        public void Register()
        {
            
        }
    }
}
