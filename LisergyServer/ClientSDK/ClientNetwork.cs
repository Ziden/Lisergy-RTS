using Game;
using Game.DataTypes;
using Game.Events.Bus;
using Game.Network;
using Game.Systems.Player;
using System;
using System.Collections.Generic;
using Telepathy;

namespace ClientSDK
{

    public class ClientNetwork : IGameNetwork, IEventListener
    {
        public event Action<BasePacket>? OnReceiveGenericPacket;

        private const int READS_PER_TICK = 20;
        private Message _msg;
        private Client _socket = new Client();
        private List<byte[]> _toSend = new List<byte[]>();

        private EventBus<BasePacket> _receivedFromServer = new EventBus<BasePacket>();

        public void On<T>(Action<T> listener) where T : BasePacket
        {
            _receivedFromServer.Register(this, listener);
        }

        public void ReceiveInput(GameId sender, BasePacket input) 
        {
            _receivedFromServer.Call(input);
            OnReceiveGenericPacket?.Invoke(input);
        }

        public void SendToPlayer(BasePacket p, params PlayerEntity[] players) 
        {
            throw new Exception("P2P Not Enabled - yet");
        }

        public void SendToServer(BasePacket p, ServerType server = ServerType.WORLD)
        {
            _toSend.Add(Serialization.FromPacketRaw(p));
        }

        public void Tick()
        {
            if (!_socket.Connected)
            {
                _socket.Connect("127.0.0.1", 1337);
                return;
            }

            while (_toSend.Count > 0)
            {
                var ev = _toSend[0];
                _toSend.RemoveAt(0);
                _socket.Send(ev);
            }

            for (var x = 0; x < READS_PER_TICK; x++)
            {
                if (!_socket.GetNextMessage(out _msg))
                    break;

                switch (_msg.eventType)
                {
                    case EventType.Connected:
                        Log.Debug("Connected to Server");
                        break;
                    case EventType.Data:
                        ReceiveInput(GameId.ZERO, Serialization.ToPacketRaw<BasePacket>(_msg.data));
                        break;
                    case EventType.Disconnected:
                        Log.Debug("Disconnected from Server");
                        break;
                }
            }

        }
    }
}
