using BaseServer.Commands;
using Game;
using Game.Network;
using NServiceBus.Logging;
using System;
using Telepathy;

namespace BaseServer.Core
{
    public abstract class SocketServer
    {
        private readonly int _port;
        private Message _pooledMessage;
        private ConsoleCommandExecutor _commandExecutor;
        protected Server _socketServer;
        public Exception ServerError { get; private set; }
        public Ticker Ticker { get; protected set; }
        public IGameLog Log { get; private set; }

        private BasePacket _packet;

        public SocketServer()
        {
            _port = GetServerType().GetPort();
            Log = new GameLog($"[Server {GetServerType()}]");
            _commandExecutor = new ConsoleCommandExecutor(null);
            _commandExecutor.RegisterCommand(new HelpCommand(_commandExecutor));
            _socketServer = new Server();
            RegisterConsoleCommands(_commandExecutor);
        }

        public void RunServer()
        {
          
            _ = _socketServer.Start(_port);
            try
            {
                Ticker = new Ticker(5);
                Log.Info($"Server Started at port {GetServerType().GetPort()}");
                Ticker.Run(RunTick);
            }
            catch (Exception e)
            {
                ServerError = e;
                Log.Error(e.ToString());
                Log.Error("Press any key to die");
                Console.ReadLine();
            }
        }

        public void Send<PacketType>(int connection, PacketType ev) where PacketType : BasePacket
        {
            Log.Debug($"Sending {ev} to {connection}");
            _socketServer.Send(connection, Serialization.FromPacket(ev));
        }

        private void RunTick()
        {
            _commandExecutor.HandleConsoleCommands();
            Tick();
            ReadSocketMessages();
        }

        public void Stop()
        {
            Ticker.Stop();
            _socketServer.Stop();
        }
        public abstract void RegisterConsoleCommands(ConsoleCommandExecutor executor);
        protected abstract bool IsAuthenticated(int connectionID);
        protected abstract bool Authenticate(BasePacket packet, int connectionID);
        public abstract void Tick();
        public abstract void Disconnect(int connectionID);
        public abstract void Connect(int connectionID);
        public abstract ServerType GetServerType();
        public abstract void ReceiveAuthenticatedPacket(int connectionId, BasePacket input);
        private void ReadSocketMessages()
        {
            while (_socketServer.GetNextMessage(out _pooledMessage))
            {
                switch (_pooledMessage.eventType)
                {
                    case EventType.Connected:
                        Connect(_pooledMessage.connectionId);
                        break;
                    case EventType.Data:

                        Log.Debug($"Received {_pooledMessage.data.Length} bytes for {_packet} from connection {_pooledMessage.connectionId}");
                        if (!IsAuthenticated(_pooledMessage.connectionId))
                        {
                            // TODO: Make not need to deserialize the whole message, check if header is AuthPacket
                            _packet = Serialization.ToPacketRaw<BasePacket>(_pooledMessage.data);
                            _packet.ConnectionID = _pooledMessage.connectionId;
                            if (!Authenticate(_packet, _pooledMessage.connectionId)) return;
                        }
                        if (_packet == null)
                        {
                            _packet = Serialization.ToPacketRaw<BasePacket>(_pooledMessage.data);
                            _packet.ConnectionID = _pooledMessage.connectionId;
                        }
                        ReceiveAuthenticatedPacket(_pooledMessage.connectionId, _packet);
                        _packet = null;
                        break;
                    case EventType.Disconnected:
                        Disconnect(_pooledMessage.connectionId);
                        break;
                }
            }
        }
    }
}
