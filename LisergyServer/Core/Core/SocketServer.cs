using BaseServer.Commands;
using Game;
using Game.Events;
using LisergyServer.Core;
using System;
using Telepathy;

namespace BaseServer.Core
{
    public abstract class SocketServer
    {
        private bool _running = false;

        protected Server _socketServer;
        private Message _msg;
        private ConsoleCommandExecutor _commandExecutor;
        public LisergyGame Game { get; protected set; }
        private readonly int _port;

        public SocketServer(int port)
        {
            _port = port;
            _commandExecutor = new ConsoleCommandExecutor(null);
            _commandExecutor.RegisterCommand(new HelpCommand(_commandExecutor));
            _socketServer = new Server();
            Serialization.LoadSerializers();
            Game = SetupGame();
            SetupServices();
            RegisterCommands(Game, _commandExecutor);
        }

        public abstract void RegisterCommands(LisergyGame game, ConsoleCommandExecutor executor);
        protected abstract ServerPlayer Auth(BaseEvent ev, int connectionID);
        public abstract void Tick();
        public abstract void Disconnect(int connectionID);
        public abstract ServerType GetServerType();
        public abstract LisergyGame SetupGame();

        public abstract void SetupServices();

        public static Ticker Ticker;

        private static readonly int TICKS_PER_SECOND = 20;
        private static readonly int TICK_DELAY_MS = 1000 / TICKS_PER_SECOND;
        private readonly DateTime _lastTick = DateTime.MinValue;

        public void Stop()
        {
            _socketServer.Stop();
            _running = false;
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

        private void RunTick()
        {
            _commandExecutor.HandleConsoleCommands();
            Tick();
            ReadSocketMessages();
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
                        byte[] message = _msg.data;

                        BaseEvent ev = Serialization.ToEventRaw(message);
                        Log.Debug($"Received {ev}");
                        ServerPlayer caller = Auth(ev, _msg.connectionId);
                        if (caller != null)
                        {
                            Game.ReceiveInput(caller, message);
                        }
                        break;
                    case EventType.Disconnected:
                        Disconnect(_msg.connectionId);
                        break;
                }
            }
        }
    }
}
