using Game;
using Game.Events;
using Game.Scheduler;
using LisergyServer.Commands;
using System;
using Telepathy;

namespace LisergyServer.Core
{
    public static class SocketServer
    {
        private static bool _running = false;

        private static readonly Server _socketServer;
        private static Message _msg;
        private static string _command;
        private static StrategyGame _game;
        private static AccountManager _accountManager;
        private static CommandExecutor _commandExecutor;

        public static bool IsRunning()
        {
            return _running;
        }

        static SocketServer()
        {
           
            _commandExecutor = new CommandExecutor();

            // TODO: Read from assembly
            _commandExecutor.RegisterCommand(new HelpCommand());
            _commandExecutor.RegisterCommand(new TileCommand());
            _commandExecutor.RegisterCommand(new TaskCommand());

            Serialization.LoadSerializers();
            _socketServer = new Server();
            _accountManager = new AccountManager(_socketServer);
            _socketServer.Start(1337);
            _running = true;
        }

        public static void RunGame(StrategyGame game)
        {
            while (_running)
            {
                try
                {
                    _commandExecutor.HandleConsoleCommands();
                    ReadSocketMessages(game);
                    GameScheduler.Tick(DateTime.UtcNow);
                } catch(Exception e)
                {
                    Console.WriteLine(e);
                    Console.WriteLine("Press any key to die");
                }
            }
        }

        private static void ReadSocketMessages(StrategyGame game)
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

                        ServerPlayer caller;
                        // normal events, we get the authenticated player or null
                        if(eventId != EventID.AUTH)
                        {
                            caller = _accountManager.GetPlayer(_msg.connectionId);

                        // for auth events we handle auth event manually
                        } else
                        {
                            var ev = Serialization.ToEvent<AuthEvent>(message);
                            ev.ConnectionID = _msg.connectionId;
                            caller = _accountManager.Authenticate(game, ev);
                        }

                        // To proccess any events we need a caller. In case it does not exists it means
                        // we cannot proccess this unauthorized event.
                        if (caller == null)
                        {
                            Game.Log.Error($"Connection {_msg.connectionId} failed auth to send event {eventId.ToString()}");
                            continue;
                        }
                        EventEmitter.CallEventFromBytes(caller, message);
                        break;
                    case EventType.Disconnected:
                        _accountManager.Disconnect(_msg.connectionId);
                        break;
                }
            }
        }
    }
}
