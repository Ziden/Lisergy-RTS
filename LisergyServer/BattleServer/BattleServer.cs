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

        public BattleServer()
        {
            _events = new NetworkEvents();
            _listener = new BattleListener();
        }

        public override StrategyGame SetupGame()
        {
            var cfg = new GameConfiguration()
            {
                WorldMaxPlayers = 10
            };
            var gameSpecs = TestSpecs.Generate();
            var world = new GameWorld();
            return new StrategyGame(cfg, gameSpecs, world);
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
