using BaseServer.Commands;
using BaseServer.Core;
using Game;
using Game.Events;
using Game.Generator;
using Game.Network;
using Game.Network.ClientPackets;
using Game.Scheduler;
using Game.Services;
using Game.World;
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

        public override void RegisterCommands(LisergyGame game, ConsoleCommandExecutor executor)
        {
            executor.RegisterCommand(new TileCommand(game));
            executor.RegisterCommand(new TaskCommand(game));
            executor.RegisterCommand(new BattlesCommand(game, _battleService));
            executor.RegisterCommand(new ServerCommand(game));
        }

        public override void Tick()
        {
            Scheduler.Tick(DateTime.UtcNow);
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
                global::Game.Log.Error($"Connection {connectionID} failed auth to send event {ev}");
                return null;
            }
            return caller;
        }

        public override LisergyGame SetupGame()
        {
            var gameSpecs = TestSpecs.Generate();
            var (sizeX, sizeY) = TestWorldGenerator.MeasureWorld(MAX_PLAYERS);
            Game = new LisergyGame(gameSpecs);

            var world = new GameWorld(MAX_PLAYERS, sizeX, sizeY);
            Game.SetWorld(world);
            world.CreateMap();
            TestWorldGenerator.PopulateWorld(world, WORLD_SEED,
                new NewbieChunkPopulator(),
                new DungeonsPopulator()
            );

            DeltaTracker.Clear();
            return Game;
        }

        public override void SetupServices()
        {
            _accountService = new AccountService(Game, _socketServer);
            _battleService = new BattleService(Game);
            _worldService = new WorldService(Game);
            _courseService = new CourseService(Game);
        }
    }
}
