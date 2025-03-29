using BaseServer;
using ClientSDK.SDKEvents;
using ClientSDK;
using Game.Engine.DataTypes;
using Game.Engine.Events.Bus;
using Game.Engine.Scheduler;
using Game.Engine;
using Godot;
using System;
using System.Collections.Generic;
using Game.Entities;
using LisergyGodotClient.Src;
using LisergyGodotClient.Src.Platform;
using LisergyGodotClient.Src.Systems;
using System.Linq;
using LisergyGodotClient.Src.Systems.Tiles;
using LisergyGodotClient.Src.Systems.Movement;
using System.Threading.Tasks;

namespace GodotClient
{
    public partial class MainNode : Node, IEventListener
    {
        public static readonly bool OFFLINE_MODE = true;

        private GodotGameObject _rootObject;
        private ClientNetwork _network;
        private List<IEventListener> _listeners = new List<IEventListener>();
        private GameScheduler _scheduler;
        private StandaloneServer _server;
        private GameStateMachine _stateMachine;
        private ClientServices _services;
        private IGamePlatform _platform;

        public override void _Ready()
        {
            GameId.INCREMENTAL_MODE = 1;

            _platform = new Windows();
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

        public override void _UnhandledInput(InputEvent @event)
        {
            base._UnhandledInput(@event);
        }

        public override void _Process(double delta)
        {
            _network?.Tick();
            if (_server != null && !_server.Multithreaded)
            {
                _server?.SingleThreadTick();
            }
            _scheduler?.Tick(DateTime.UtcNow);
        }

        private void OnApplicationQuit()
        {
            _network?.Disconnect();
            _server?.Dispose();
        }

        private void OnGameStarted(GameStartedEvent ev)
        {
            GodotLog.SetupLog(ev.Game.Log, ConsoleColor.Yellow);
            GodotLog.SetupLog(ClientServices.ServerSdk.Log, ConsoleColor.Gray);
            _listeners.Add(new EntityPositionListener());
            _listeners.Add(new TileListener());
            _listeners.Add(new TileRenderingListener());
            _scheduler = ClientServices.ServerSdk.Game.Scheduler as GameScheduler;
        }

        public void SetupViews()
        {
#if DEBUG
            _rootObject.Node.ChildEnteredTree += (node) =>
            {
                var name = node.GetScript().Obj != null ? node.GetScript().Obj.GetType().Name : node.SceneFilePath.Split("/").Last();
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
            Telepathy.Logger.Log = ClientServices.Log.Info;
            Telepathy.Logger.LogWarning = ClientServices.Log.Info;
            Telepathy.Logger.LogError = ClientServices.Log.Error;
        }

    }
}
