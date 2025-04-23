using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseServer;
using ClientSDK;
using ClientSDK.Autoplay;
using ClientSDK.SDKEvents;
using Game.Engine;
using Game.Engine.DataTypes;
using Game.Engine.Events.Bus;
using Game.Engine.Scheduler;
using Game.Entities;
using Godot;
using LisergyGodotClient.Data;
using LisergyGodotClient.Src;
using LisergyGodotClient.Src.Platform;
using LisergyGodotClient.Src.Systems;
using LisergyGodotClient.Src.Systems.Movement;
using LisergyGodotClient.Src.Systems.Tiles;
using Telepathy;

namespace GodotClient;

public partial class MainNode : Node, IEventListener
{
	public static readonly bool OFFLINE_MODE = true;
	private ClientNetwork _network;
	private IGamePlatform _platform;
	
	private GodotGameObject _rootObject;
	private GameScheduler _scheduler;
	private StandaloneServer _server;
	private ClientServices _services;
	private GameStateMachine _stateMachine;

	public override void _Ready()
	{
		GameId.INCREMENTAL_MODE = 1;
#if WINDOWS
            _platform = new Windows();
#elif OSX
		_platform = new Osx();
#endif
		_platform.Initialize();
		_rootObject = new GodotGameObject(this);
		
		_services = new ClientServices(_rootObject);
		SetupViews();
		ConfigureGodot();
		ClientServices.ServerSdk.ClientEvents.On<GameStartedEvent>(this, OnGameStarted);
		_network = ClientServices.ServerSdk.Network as ClientNetwork;
		Serialization.LoadSerializers();
		_stateMachine = new GameStateMachine();
		if (OFFLINE_MODE)
		{
			_server = new StandaloneServer();
			_server.Multithreaded = false;
			_server.Start();
		}
	}
	
	private void OnGameStarted(GameStartedEvent ev)
	{
		GodotLog.SetupLog(ev.Game.Log, ConsoleColor.Yellow);
		GodotLog.SetupLog(ClientServices.ServerSdk.Log, ConsoleColor.Gray);
		_scheduler = ClientServices.ServerSdk.Game.Scheduler as GameScheduler;
		ListenersRegistration.LoadAutoRegisterListeners();
	}
	
	public override void _Process(double delta)
	{
		_network?.Tick();
		if (_server != null && !_server.Multithreaded) _server?.SingleThreadTick();
		_scheduler?.Tick(DateTime.UtcNow);
		ClientServices.ServerSdk.Server.Actions.TickAutoplay();
	}

	private void OnApplicationQuit()
	{
		_network?.Disconnect();
		_server?.Dispose();
	}

	public void SetupViews()
	{
#if DEBUG
		_rootObject.Node.ChildEnteredTree += node =>
		{
			var name = node.GetScript().Obj != null
				? node.GetScript().Obj.GetType().Name
				: node.SceneFilePath.Split("/").Last();
			ClientServices.Log.Debug("Entered scene: " + name);
		};
#endif
		ClientServices.ServerSdk.Server.Views.RegisterView(
			EntityType.Tile, e => new TileView(e, ClientServices.ServerSdk));

		ClientServices.ServerSdk.Server.Views.RegisterView(
			EntityType.Dungeon, e => new DungeonView(e, ClientServices.ServerSdk));

		ClientServices.ServerSdk.Server.Views.RegisterView(
			EntityType.Party, e => new MovablevView(e, ClientServices.ServerSdk));
	}

	public static void ConfigureGodot()
	{
		TaskScheduler.UnobservedTaskException += (sender, e) =>
		{
			ClientServices.Analytics.TrackError(e.Exception);
			throw e.Exception;
		};
		AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
		{
			var ex = e.ExceptionObject as Exception;
			ClientServices.Analytics.TrackError(ex);
			throw ex;
		};
		Engine.PrintErrorMessages = true;
		Engine.MaxFps = 60;
		OS.SetLowProcessorUsageMode(true);
		Logger.Log = ClientServices.Log.Info;
		Logger.LogWarning = ClientServices.Log.Info;
		Logger.LogError = ClientServices.Log.Error;
	}
}