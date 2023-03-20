using BaseServer.Commands;
using BaseServer.Core;
using BattleServer;
using Game;
using Game.Events;
using Game.Generator;
using Game.Listeners;
using Game.Network;
using Game.Network.ClientPackets;
using Game.Scheduler;
using GameDataTest;
using LisergyServer.Commands;
using LisergyServer.Core;
using System;

namespace MapServer
{
    public class MapSocketServer : SocketServer
    {
        private static readonly int MAX_PLAYERS = 40;

        private static int WORLD_SEED = 12345;
        // TODO: Move to account server
        private AccountService _accountService;
        // TODO: Move to battle server
        private BattleService _battleService;
        private WorldService _worldService;
        private CourseService _courseService;

        public override ServerType GetServerType() => ServerType.MAP;

        public MapSocketServer(int port) : base(port)
        {
        }

        public override void RegisterCommands(StrategyGame game, CommandExecutor executor)
        {
            executor.RegisterCommand(new TileCommand(game));
            executor.RegisterCommand(new TaskCommand(game));
            executor.RegisterCommand(new BattlesCommand(game, _battleService));
            executor.RegisterCommand(new ServerCommand(game));
        }

        public override void Tick()
        {
            GameScheduler.Tick(DateTime.UtcNow);
        }

        public override void Disconnect(int connectionID)
        {
            _accountService.Disconnect(connectionID);
        }

        protected override ServerPlayer Auth(BaseEvent ev, int connectionID)
        {
            ServerPlayer caller;
            if (!(ev is AuthPacket))
            {
                caller = _accountService.GetPlayer(connectionID);
            }
            else
            {
                ev.ConnectionID = connectionID;
                caller = _accountService.Authenticate((AuthPacket)ev);
            }
            if (caller == null)
            {
                Game.Log.Error($"Connection {connectionID} failed auth to send event {ev}");
                return null;
            }
            return caller;
        }

        public override StrategyGame SetupGame()
        {
            var gameSpecs = TestSpecs.Generate();
            _game = new StrategyGame(gameSpecs, null);

            var (sizeX, sizeY) = TestWorldGenerator.MeasureWorld(MAX_PLAYERS);

            _game.World = new GameWorld(MAX_PLAYERS, sizeX, sizeY);
            TestWorldGenerator.PopulateWorld(_game.World, WORLD_SEED,
                new NewbieChunkPopulator(),
                new DungeonsPopulator()
            );
            DeltaTracker.Clear();
            return _game;
        }

        public override void SetupServices()
        {
            _accountService = new AccountService(_game, _socketServer);
            _battleService = new BattleService(_game);
            _worldService = new WorldService(_game);
            _courseService = new CourseService(_game);
        }
    }
}
