using BaseServer.Commands;
using Game;
using Game.Engine;
using Game.Engine.DataTypes;
using Game.Engine.Network;
using Game.Engine.Scheduler;
using Game.Events.ServerEvents;
using Game.Services;
using LisergyServer.Commands;
using LisergyServer.Core;
using System;

namespace MapServer
{
    /// <summary>
    /// Unified one server for everything, for now
    /// </summary>
    public class WorldServer : BaseHandshackedServer
    {
        public LisergyGame Game { get; private set; }
        private GameScheduler _gameTaskScheduler;
        private GameServerNetwork _network;
        private BattleService _battleService;
        private WorldService _worldService;
        private byte[] _gameSpecs;

        public override ServerType GetServerType() => ServerType.WORLD;

        public WorldServer(LisergyGame game) : base()
        {
            Serialization.LoadSerializers();
            Game = game;
            _gameTaskScheduler = game.Scheduler as GameScheduler;
            _network = game.Network as GameServerNetwork;
            _battleService = new BattleService(Game);
            _worldService = new WorldService(Game);
            _gameSpecs = Serialization.FromBasePacket(new GameSpecPacket(game));
            _network.OnOutgoingPacket += SendPacketToPlayer;
        }

        public override void RegisterConsoleCommands(ConsoleCommandExecutor executor)
        {
            executor.RegisterCommand(new TileCommand(Game));
            executor.RegisterCommand(new TaskCommand(Game));
            executor.RegisterCommand(new BattlesCommand(Game, _battleService));
            executor.RegisterCommand(new ServerCommand(Game));
        }

        public override void Tick()
        {
            _gameTaskScheduler.Tick(DateTime.UtcNow);
        }

        public override void OnReceiveAuthenticatedPacket(in GameId player, BasePacket packet)
        {
            Game.Network.ReceiveInput(player, packet);
        }

        public override void OnAuthenticated(ConnectedPlayer player)
        {
            player.Send(_gameSpecs);
        }
    }
}
