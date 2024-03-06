using Game.Engine.DataTypes;
using Game.Engine.Events.Bus;
using Game.Engine.Network;
using Game.Systems.Player;
using Game.World;
using System;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Game.Engine
{
    public enum ServerType
    {
        /// <summary>
        /// Handles the game world, entities and components
        /// </summary>
        WORLD,

        /// <summary>
        /// Handles authentication. Can generate tokens so the player can handshake with other servers
        /// </summary>
        ACCOUNT,

        /// <summary>
        /// Handles player chat. 
        /// </summary>
        CHAT,

        /// <summary>
        /// Proccess battles from a queue of battles to be processed
        /// </summary>
        BATTLE,
    }

    public static class ServerNetworkExt
    {
        public static int PORT_START = 1337;

        public static int GetDefaultPort(this ServerType server) => PORT_START + (int)server;
    }


    /// <summary>
    /// Network interface to send and receive packets.
    /// </summary>
    public interface IGameNetwork
    {
        /// <summary>
        /// Receive and proccess an input packet
        /// </summary>

        void ReceiveInput(GameId sender, BasePacket input);

        /// <summary>
        /// Listens for packets being received
        /// </summary>

        public void On<T>(Action<T> listener) where T : BasePacket;

        /// <summary>
        /// Sends a packet to player
        /// </summary>

        public void SendToPlayer<PacketType>(PacketType p, params PlayerEntity[] players) where PacketType : BasePacket;

        /// <summary>
        /// Sends a packet to server.
        /// </summary>

        public void SendToServer(BasePacket p, ServerType server = ServerType.WORLD);
    }

    public class GameServerNetwork : IGameNetwork, IEventListener
    {
        public Action<GameId, BasePacket> OnOutgoingPacket;

        public EventBus<BasePacket> IncomingPackets { get; } = new EventBus<BasePacket>();

        private IGame _game;

        public GameServerNetwork(IGame game)
        {
            _game = game;
        }


        public void On<T>(Action<T> listener) where T : BasePacket
        {
            IncomingPackets.Register(this, listener);
        }


        public void ReceiveInput(GameId sender, BasePacket input)
        {
            input.Sender = _game.Players[sender] ?? new PlayerEntity(new PlayerProfile(sender), _game);
            IncomingPackets.Call(input);
            _game.Entities.DeltaCompression.SendDeltaPackets(input.Sender);
        }


        public void SendToServer(BasePacket p, ServerType server)
        {
            IncomingPackets.Call(p); // Hack for local server for now
        }


        public void SendToPlayer<PacketType>(PacketType p, params PlayerEntity[] players) where PacketType : BasePacket
        {
            foreach (var player in players)
            {
                if (player == null) continue;
                OnOutgoingPacket?.Invoke(player.EntityId, p);
            }
        }
    }
}
