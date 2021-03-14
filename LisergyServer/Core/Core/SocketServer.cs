using Game;
using Game.Events;
using LisergyServer.Commands;
using System;
using Telepathy;

namespace LisergyServer.Core
{
    public abstract class SocketServer
    {
        private bool _running = false;

        protected readonly Server _socketServer;
        private Message _msg;
        private CommandExecutor _commandExecutor;
        protected StrategyGame _game;

        public SocketServer()
        {
            _commandExecutor = new CommandExecutor();
            _commandExecutor.RegisterCommand(new HelpCommand(_commandExecutor));
            RegisterCommands(_commandExecutor);
            Serialization.LoadSerializers();
            _socketServer = new Server();
            _game = SetupGame();
        }

        public abstract void RegisterCommands(CommandExecutor executor);
        protected abstract ServerPlayer Auth(EventID eventId, int connectionID, byte[] message);
        public abstract void Tick();
        public abstract void Disconnect(int connectionID);
        public abstract ServerType GetServerType();
        public abstract StrategyGame SetupGame();

        public void Initialize(int port)
        {
            _socketServer.Start(port);
            _running = true;
        }

        public void Stop()
        {
            _socketServer.Stop();
            _running = false;
        }

        public void RunServer()
        {
            while (_running)
            {
                try
                {
                    _commandExecutor.HandleConsoleCommands();
                    Tick();
                    ReadSocketMessages();
                } catch(Exception e)
                {
                    Console.WriteLine(e);
                    Console.WriteLine("Press any key to die");
                }
            }
        }

        private void ReadSocketMessages()
        {
            while (_socketServer.GetNextMessage(out _msg))
            {
                switch (_msg.eventType)
                {
                    case EventType.Connected:
                        Console.WriteLine("New Connection Received");
                        break;
                    case EventType.Data:
                        var message = _msg.data;
                        var eventId = (EventID)message[0];
                        Game.Log.Debug($"Received {eventId.ToString()} event");
                        var caller = Auth(eventId, _msg.connectionId, _msg.data);
                        if(caller != null)
                            EventEmitter.CallEventFromBytes(caller, message);
                        break;
                    case EventType.Disconnected:
                        Disconnect(_msg.connectionId);
                        break;
                }
            }
        }
    }
}
