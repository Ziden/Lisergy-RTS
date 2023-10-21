using Game;
using Game.DataTypes;
using Game.Events.Bus;
using Game.Network;
using Game.Systems.Player;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Telepathy;

[assembly: InternalsVisibleTo("ServerTests")]
namespace ClientSDK
{
    public class ClientNetwork : IGameNetwork, IEventListener
    {
        public event Action<BasePacket>? OnReceiveGenericPacket;

        private bool _enabled = true;
        internal const int READS_PER_TICK = 20;
        internal Message _msg;
        private IGameLog _log;
        internal Dictionary<ServerType, Client> _connections = new Dictionary<ServerType, Client>();
        internal Dictionary<ServerType, Queue<byte[]>> _toSend = new Dictionary<ServerType, Queue<byte[]>>();
        internal EventBus<BasePacket> _receivedFromServer = new EventBus<BasePacket>();

        public ClientNetwork(IGameLog log, params ServerType[] connections)
        {
            _log = log;
            foreach(var server in connections)
            {
                _connections[server] = new Client();
                _toSend[server] = new Queue<byte[]>();
            }
        }

        public void On<T>(Action<T> listener) where T : BasePacket
        {
            _receivedFromServer.Register(this, listener);
        }

        public void ReceiveInput(GameId sender, BasePacket input) 
        {
            _receivedFromServer.Call(input);
            OnReceiveGenericPacket?.Invoke(input);
        }

        public void ReceiveServerMessage(ServerType server, BasePacket input)
        {
            ReceiveInput(GameId.ZERO, input);
        }

        public void SendToPlayer<PacketType>(PacketType p, params PlayerEntity[] players) where PacketType : BasePacket
        {
            throw new Exception("P2P Not Enabled - yet");
        }

        public void SendToServer(BasePacket p, ServerType server = ServerType.WORLD)
        {
            _enabled = true;
            _toSend[server].Enqueue(Serialization.FromPacketRaw(p));
        }

        public void Disconnect()
        {
            _log.Info("Client disconnecting");
            _enabled = false;
            foreach(var client in _connections.Values) client.Disconnect();
        }

        public void Tick()
        {
            if (!_enabled) return;
            foreach (var (server, socket) in _connections)
            {
                if (!socket.Connected)
                {
                    socket.Connect("127.0.0.1", server.GetPort());
                    return;
                }

                while (_toSend[server].TryDequeue(out var data))
                {
                    socket.Send(data);
                }

                for (var x = 0; x < READS_PER_TICK; x++)
                {
                    if (!socket.GetNextMessage(out _msg))
                        break;

                    switch (_msg.eventType)
                    {
                        case EventType.Connected:
                            _log.Debug("Connected to Server");
                            break;
                        case EventType.Data:
                            ReceiveServerMessage(server, Serialization.ToPacketRaw<BasePacket>(_msg.data));
                            break;
                        case EventType.Disconnected:
                            _log.Debug("Disconnected from Server");
                            break;
                    }
                }
            }
        }
    }
}
