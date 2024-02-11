using Assets.Code;
using Assets.Code.Assets.Code;
using Assets.Code.Assets.Code.Assets;
using Assets.Code.Assets.Code.Audio;
using Assets.Code.Assets.Code.Runtime;
using Assets.Code.Assets.Code.UIScreens.Base;
using Assets.Code.Views;
using Assets.Code.World;
using ClientSDK;
using ClientSDK.SDKEvents;
using Game;
using Game.Events.Bus;
using Game.Scheduler;
using Game.Systems.Building;
using Game.Systems.Dungeon;
using Game.Systems.Party;
using Game.Tile;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour, IEventListener
{
    public static readonly bool OFFLINE_MODE = false;

    private GameClient _client;
    private ClientNetwork _network;
    private GameStateMachine _stateMachine;
    private List<IEventListener> _listeners = new List<IEventListener>();
    private GameScheduler _scheduler;

    void Awake()
    {
        Debug.Log("Main Awake");
        _client = new GameClient();
        _client.ClientEvents.Register<GameStartedEvent>(this, OnGameStarted);
        _network = _client.Network as ClientNetwork;
        SetupViews();
        ConfigureUnity();
        SetupServices();
        Serialization.LoadSerializers();
    }

    void Start()
    {
        Debug.Log("Starting Main Behaviour");
        DontDestroyOnLoad(gameObject);
        UnityServicesContainer.OnSceneLoaded();
        _stateMachine = new GameStateMachine(_client);
    }

    void Update()
    {
        _network.Tick();
        _scheduler?.Tick(DateTime.UtcNow);
    }

    private void OnApplicationQuit()
    {
        _network.Disconnect();
        //_client.Game.Entities.Dispose();
    }

    private void SetupLog(IGameLog ilog)
    {
        GameLog log = (GameLog)ilog;
        log._Debug = Debug.Log;
        log._Info = Debug.Log;
        log._Error = Debug.LogError;
    }

    /// <summary>
    /// Registers event listeners to change client behaviour
    /// </summary>
    private void OnGameStarted(GameStartedEvent ev)
    {
        SetupLog(ev.Game.Log);
        // SetupLog(_client.Log); // SDK LOGS
        _listeners.Add(new FogOfWarListener(_client));
        _listeners.Add(new EntityPositionListener(_client));
        _listeners.Add(new IndicatorSelectedTileListener(_client));
        _listeners.Add(new IndicatorSelectedPartyListener(_client));
        _listeners.Add(new PartyMovementListener(_client));
        _listeners.Add(new BattleGroupListener(_client));
        _listeners.Add(new HarvestingComponentListener(_client));
        _listeners.Add(new HarvestingViewListener(_client));
        _listeners.Add(new BattleGroupUnitListener(_client));
        _scheduler = _client.Game.Scheduler as GameScheduler;
    }

    public void SetupViews()
    {
        _client.Modules.Views.RegisterView<PartyEntity, PartyView>();
        _client.Modules.Views.RegisterView<DungeonEntity, DungeonView>();
        _client.Modules.Views.RegisterView<TileEntity, TileView>();
        _client.Modules.Views.RegisterView<PlayerBuildingEntity, PlayerBuildingView>();
    }

    public void SetupServices()
    {
        UnityServicesContainer.Register<IInputService, InputService>(gameObject?.AddComponent<InputService>());
        UnityServicesContainer.Register<IUiService, UiService>(new UiService(_client));
        UnityServicesContainer.Register<IAudioService, AudioService>(new AudioService(_client));
        UnityServicesContainer.Register<INotificationService, NotificationService>(new NotificationService(_client));
        UnityServicesContainer.Register<IAssetService, AssetService>(new AssetService());
        UnityServicesContainer.Register<IServerModules, ServerModules>(_client.Modules as ServerModules);
        UnityServicesContainer.Register<IVfxService, VfxService>(new VfxService(_client));
    }

    public static void ConfigureUnity()
    {
        Application.runInBackground = true;
        Telepathy.Logger.Log = Debug.Log;
        Telepathy.Logger.LogWarning = Debug.LogWarning;
        Telepathy.Logger.LogError = Debug.LogError;
    }
}