using System;
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

namespace MapServer;

/// <summary>
///     Unified one server for everything, for now
/// </summary>
public class WorldServer : BaseHandshackedServer
{
	private readonly BattleService _battleService;
	private readonly byte[] _gameSpecs; // MOVE to a data service
	private readonly GameScheduler _gameTaskScheduler;
	private readonly GameServerNetwork _network;
	private WorldService _worldService;

	public WorldServer(LisergyGame game)
	{
		Serialization.LoadSerializers();
		Game = game;
		_gameTaskScheduler = game.Scheduler as GameScheduler;
		_network = game.Network as GameServerNetwork;
		_battleService = new BattleService(Game);
		_worldService = new WorldService(Game);
		var specPacket = new GameSpecPacket(game);
		specPacket.OnBeforeSerialize();
		_gameSpecs = Serialization.FromAnyType(specPacket).ToArray();
		_network.OnOutgoingPacket += SendPacketToPlayer;
	}

	public LisergyGame Game { get; }

	public override ServerType GetServerType()
	{
		return ServerType.WORLD;
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
		player.SendBytes(_gameSpecs);
	}
}