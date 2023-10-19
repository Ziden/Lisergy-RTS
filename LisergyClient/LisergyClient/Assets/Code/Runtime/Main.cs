using Assets.Code;
using Assets.Code.Assets.Code;
using Assets.Code.Assets.Code.Assets;
using Assets.Code.Assets.Code.Audio;
using Assets.Code.Assets.Code.Runtime;
using Assets.Code.Assets.Code.UIScreens.Base;
using Assets.Code.UI;
using Assets.Code.Views;
using Assets.Code.World;
using ClientSDK;
using ClientSDK.SDKEvents;
using Game;
using Game.Events.Bus;
using Game.Systems.Building;
using Game.Systems.Dungeon;
using Game.Systems.Party;
using Game.Tile;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour, IEventListener
{
    public static readonly bool OFFLINE_MODE = true;

    private GameClient _client;
    private ClientNetwork _network;
    private GameStateMachine _stateMachine;
    private List<IEventListener> _listeners = new List<IEventListener>();

    void Awake()
    {
        Debug.Log("Main Awake");
        _client = new GameClient();
        _client.ClientEvents.Register<GameStartedEvent>(this, SetupClientSystemListeners);
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
    }

    private void OnApplicationQuit()
    {
        // Disposes
    }

    /// <summary>
    /// Registers event listeners to change client behaviour
    /// </summary>
    private void SetupClientSystemListeners(GameStartedEvent ev)
    {
        _listeners.Add(new FogOfWarListener(_client)); 
        _listeners.Add(new MapPlacementComponentListener(_client));
        _listeners.Add(new IndicatorSelectedTileListener(_client));
        _listeners.Add(new IndicatorSelectedEntityListener(_client));
        _listeners.Add(new MapAnimationListener(_client));
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
        UnityServicesContainer.Register<IInputService, InputService>(CreateInputService());
        UnityServicesContainer.Register<IScreenService, ScreenService>(new ScreenService(_client));
        UnityServicesContainer.Register<IAudioService, AudioService>(new AudioService(_client));
        UnityServicesContainer.Register<INotificationService, NotificationService>(new NotificationService(_client));
        UnityServicesContainer.Register<IAssetService, AssetService>(new AssetService());
        UnityServicesContainer.Register<IServerModules, ServerModules>(_client.Modules as ServerModules);
    }

    InputService CreateInputService()
    {
        return gameObject?.AddComponent<InputService>();
    }


    public static void ConfigureUnity()
    {
        Application.runInBackground = true;
        Telepathy.Logger.Log = Debug.Log;
        Telepathy.Logger.LogWarning = Debug.LogWarning;
        Telepathy.Logger.LogError = Debug.LogError;
        Log._Debug = Debug.Log;
        Log._Error = Debug.LogError;
        Log._Info = Debug.Log;
        TrackAsyncErrors();
    }

    private static void TrackAsyncErrors()
    {
        //TaskScheduler.UnobservedTaskException += (s, a) => Debug.LogException(a?.Exception);
        //AppDomain.CurrentDomain.FirstChanceException += (sender, args) => Debug.LogException(args.Exception);
        //AppDomain.CurrentDomain.UnhandledException += (sender, args) => Debug.LogException((Exception)args.ExceptionObject);
    }
}