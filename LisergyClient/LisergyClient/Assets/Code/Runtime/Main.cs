using Assets.Code;
using Assets.Code.Assets.Code;
using Assets.Code.Assets.Code.Assets;
using Assets.Code.Assets.Code.Audio;
using Assets.Code.Assets.Code.Runtime;
using Assets.Code.Assets.Code.UIScreens.Base;
using Assets.Code.Views;
using Assets.Code.World;
using ClientSDK;
using Game;
using Game.Systems.Building;
using Game.Systems.Dungeon;
using Game.Systems.Party;
using Game.Tile;
using System;
using System.Threading.Tasks;
using UnityEngine;

public class Main : MonoBehaviour
{
    public static LocalPlayer LocalPlayer { get; private set; } // TODO: Delete

    private GameClient _client;
    private ClientNetwork _network;
    private GameStateMachine _stateMachine;

    void Awake()
    {
        Debug.Log("Main Awake");
        _client = new GameClient();
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
        ServiceContainer.OnSceneLoaded();
        _stateMachine = new GameStateMachine(_client);
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
        TaskScheduler.UnobservedTaskException += (s, a) => Debug.LogException(a?.Exception);
        AppDomain.CurrentDomain.FirstChanceException += (sender, args) => Debug.LogException(args.Exception);
        AppDomain.CurrentDomain.UnhandledException += (sender, args) => Debug.LogException((Exception)args.ExceptionObject);
    }

    void Update()
    {
        _network.Tick();
    }

    private void OnApplicationQuit()
    {
        // Disposes
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
        ServiceContainer.Register<IInputService, InputService>(CreateInputService());
        ServiceContainer.Register<IScreenService, ScreenService>(new ScreenService(_client));
        ServiceContainer.Register<IAudioService, AudioService>(new AudioService(_client));
        ServiceContainer.Register<INotificationService, NotificationService>(new NotificationService(_client));
        ServiceContainer.Register<IAssetService, AssetService>(new AssetService());
        ServiceContainer.Register<IServerModules, ServerModules>(_client.Modules as ServerModules);
    }

    InputService CreateInputService()
    {
        return gameObject?.AddComponent<InputService>();
    }
}