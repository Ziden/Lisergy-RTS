using BaseServer.Commands;
using Game;
using Game.Network;
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
        public Ticker Ticker { get; protected set; }

        public SocketServer(int port)
        {
            _port = port;
            _commandExecutor = new ConsoleCommandExecutor(null);
            _commandExecutor.RegisterCommand(new HelpCommand(_commandExecutor));
            _socketServer = new Server();
            RegisterConsoleCommands(_commandExecutor);
        }

        public void RunServer()
        {
            Log.Info("Starting Server");
            _ = _socketServer.Start(_port);
            try
            {
                Ticker = new Ticker(5);
                Ticker.Run(RunTick);
            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
                Log.Error("Press any key to die");
            }
        }

        public void Send<PacketType>(int connection, PacketType ev) where PacketType : BasePacket
        {
            Log.Debug($"Sending {ev} to {connection}");
            _socketServer.Send(connection, Serialization.FromPacket<PacketType>(ev));
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
        protected abstract bool IsAuthenticated(BasePacket ev, int connectionID);
        public abstract void Tick();
        public abstract void Disconnect(int connectionID);
        public abstract void Connect(int connectionID);
        public abstract ServerType GetServerType();
        public abstract void HandleInputPacket(int connectionId, BasePacket input);
        private void ReadSocketMessages()
        {
            while (_socketServer.GetNextMessage(out _pooledMessage))
            {
                switch (_pooledMessage.eventType)
                {
                    case EventType.Connected:
                        Console.WriteLine("New Connection Received");
                        Connect(_pooledMessage.connectionId);
                        break;
                    case EventType.Data:
                        byte[] message = _pooledMessage.data;
                        var ev = Serialization.ToPacketRaw<BasePacket>(message);
                        ev.ConnectionID = _pooledMessage.connectionId;
                        Log.Debug($"Received {message.Length} bytes for {ev} from connection {_pooledMessage.connectionId}");
                        if (IsAuthenticated(ev, _pooledMessage.connectionId))
                        {
                            HandleInputPacket(_pooledMessage.connectionId, ev);
                        }
                        break;
                    case EventType.Disconnected:
                        Disconnect(_pooledMessage.connectionId);
                        break;
                }
            }
        }
    }
}
