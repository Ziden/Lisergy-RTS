using Game;
using Game.Events;
using Game.Events.ServerEvents;
using LegendsServer.Core;
using System;
using System.Collections.Generic;
using System.Text;
using Telepathy;

namespace LisergyServer.Core
{
    public class SocketServer
    {
        private static bool Running = false;
        private static readonly Server server;
        private static Message msg;
        private static StrategyGame game;
        private static Dictionary<int, ServerPlayer> onlinePlayers = new Dictionary<int, ServerPlayer>();
        private static PlayerManager playerManager;
        
        static SocketServer()
        {
            Serialization.LoadSerializers();
            server = new Server();
            playerManager = new PlayerManager(server);
            server.Start(1337);
            Running = true;
        }

        public static void ServerLoop(StrategyGame game)
        {
            while (Running)
            {
                ReadSocketMessages(game);
            }
        }

        private static void ReadSocketMessages(StrategyGame game)
        {
            while (server.GetNextMessage(out msg))
            {
                switch (msg.eventType)
                {
                    case EventType.Connected:
                        Console.WriteLine("New Connection Received");
                        break;
                    case EventType.Data:
                        var message = msg.data;
                        var eventId = (EventID)message[0];
                        
                        Game.Log.Debug($"Received {eventId.ToString()} event");

                        // AUTH //
                        ServerPlayer caller;
                        if(eventId != EventID.AUTH)
                        {
                            caller = playerManager.GetPlayer(msg.connectionId);
                        } else
                        {
                            var ev = Serialization.ToEvent<AuthEvent>(message);
                            ev.ConnectionID = msg.connectionId;
                            caller = playerManager.Authenticate(game, ev);
                        }

                        if (caller == null)
                        {
                            Game.Log.Error($"Connection {msg.connectionId} failed auth to send event {eventId.ToString()}");
                            continue;
                        }
                        EventEmitter.CallEventFromBytes(caller, message);
                        break;
                    case EventType.Disconnected:
                        playerManager.Disconnect(msg.connectionId);
                        break;
                }
            }
        }
    }
}
