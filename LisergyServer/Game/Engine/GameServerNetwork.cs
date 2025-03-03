using Game.Engine.DataTypes;
using Game.Engine.Events.Bus;
using Game.Engine.Network;
using Game.Entities;
using Game.Systems.Player;
using System;

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
        void SetupGame(IGame game);

        DeltaCompression DeltaCompression { get; }
        /// <summary>
        /// Receive and proccess an input packet
        /// </summary>

        void ReceiveInput(GameId sender, BasePacket input);

        /// <summary>
        /// Listens for packets being received
        /// </summary>

        public void OnInput<T>(Action<T> listener) where T : BasePacket;

        /// <summary>
        /// Sends a packet to player
        /// </summary>

        public void SendToPlayer<PacketType>(PacketType p, params GameId[] players) where PacketType : BasePacket;

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
        private DeltaCompression _deltas;

        public DeltaCompression DeltaCompression => _deltas;
        public GameServerNetwork(IGame game)
        {
            _game = game;
            _deltas = new DeltaCompression(game);
        }


        public void OnInput<T>(Action<T> listener) where T : BasePacket
        {
            IncomingPackets.On(this, listener);
        }


        public void ReceiveInput(GameId sender, BasePacket input)
        {
            CheckUser(sender, input);
            IncomingPackets.Call(input);
            if (input is IGameCommand command)
            {
                command.Execute(_game);
            }
            if (_game.Players[sender] != null)
            {
                _game.Network.DeltaCompression.SendAllModifiedEntities(sender);
            }
        }

        // TODO: Should this be here?
        private void CheckUser(GameId sender, BasePacket input)
        {
            input.SenderPlayerId = sender;
            var senderEntity = _game.Entities[sender];
            if (senderEntity == null)
            {
                senderEntity = _game.Entities.CreateEntity(EntityType.Player, GameId.ZERO, sender);
                var profile = new PlayerProfileComponent(sender);
                senderEntity.Save(profile);
            }
            var senderPlayer = _game.Players[sender];
            if (input.Sender == null && senderPlayer != null)
            {
                input.Sender = senderPlayer;
            }
        }

        public void SendToServer(BasePacket p, ServerType server)
        {
            IncomingPackets.Call(p); // Hack for local server for now
        }


        public void SendToPlayer<PacketType>(PacketType p, params GameId[] players) where PacketType : BasePacket
        {
            foreach (var player in players)
            {
                if (player == null) continue;
                OnOutgoingPacket?.Invoke(player, p);
            }
        }

        public void SetupGame(IGame game)
        {
            _game = game;
        }
    }
}
