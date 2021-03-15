using BattleServer;
using BattleServer.Core;
using Game;
using Game.Events;
using GameDataTest;
using LisergyServer.Commands;
using LisergyServer.Core;
using System;

namespace BattleServer
{
    public class BattleServer : SocketServer
    {
        private BattleListener _listener;
        private NetworkEvents _events;

        public BattleServer(int port): base(port)
        {
            _events = new NetworkEvents();
            _listener = new BattleListener();
        }

        public override StrategyGame SetupGame()
        {
            var gameSpecs = TestSpecs.Generate();
            return new StrategyGame(gameSpecs, null);
        }

        protected override ServerPlayer Auth(EventID eventId, int connectionID, byte[] message)
        {
            Console.WriteLine("New remote server connected");
            return new RemoteServerEntity(connectionID, _socketServer);
        }

        public override void Tick()
        {

        }

        public override void Disconnect(int connectionID)
        {
        }

        public override void RegisterCommands(CommandExecutor executor)
        {
        }

        public override ServerType GetServerType() => ServerType.BATTLE;
    }
}
