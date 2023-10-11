using Assets.Code;
using Assets.Code.Assets.Code;
using Assets.Code.Assets.Code.Assets;
using Assets.Code.Assets.Code.Audio;
using Assets.Code.Assets.Code.Runtime;
using Assets.Code.Assets.Code.UIScreens;
using Assets.Code.Assets.Code.UIScreens.Base;
using Game;
using Game.Events;
using Game.Events.Bus;
using Game.Network;
using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public class MainBehaviour : MonoBehaviour
{
    private static MainBehaviour _instance;

    public static Networking Networking { get; private set; }
    public static LocalPlayer LocalPlayer { get; private set; }
    public static EventBus<BasePacket> ServerPackets { get; set; }

    private ServerListener _serverPacketListener;

    private GameStateMachine _stateMachine;

    void Start()
    {
        Debug.Log("Starting Main Behaviour");
        DontDestroyOnLoad(this.gameObject);
        ServiceContainer.OnSceneLoaded();
        ClientEvents.OnPlayerLogin += OnPlayerLogin;
        _stateMachine = new GameStateMachine();
    }

    public static async Task RunAsync(Action a, float seconds)
    {
        await Task.Delay((int)(seconds * 1000));
        a();
    }

    public static void RunCoroutine(IEnumerator coroutine)
    {
        _instance.StartCoroutine(coroutine);
    }

    public static IEnumerator Coroutine(Action a, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        a();
    }

    public static GameObject CreatePrefab(UnityEngine.Object prefab)
    {
        return Instantiate(prefab) as GameObject;
    }

    public static void DestroyObject(GameObject obj)
    {
        Destroy(obj);
    }

    public void OnPlayerLogin(LocalPlayer player)
    {
        MainBehaviour.LocalPlayer = player;
        player.SetupLocalPlayer();
    }

    public static void ConfigureUnity()
    {
        Application.runInBackground = true;
        Telepathy.Logger.Log = Debug.Log;
        Telepathy.Logger.LogWarning = Debug.LogWarning;
        Telepathy.Logger.LogError = Debug.LogError;
        Game.Log.Debug = Debug.Log;
        Game.Log.Error = Debug.LogError;
        Game.Log.Info = Debug.Log;
        TrackAsyncErrors();
    }

    private static void TrackAsyncErrors()
    {
        TaskScheduler.UnobservedTaskException += (s, a) => Debug.LogException(a?.Exception);
        AppDomain.CurrentDomain.FirstChanceException += (sender, args) => Debug.LogException(args.Exception);
        AppDomain.CurrentDomain.UnhandledException += (sender, args) => Debug.LogException((Exception)args.ExceptionObject);
    }

    void Awake()
    {
        _instance = this;
        Networking = new Networking();
        ServerPackets = new EventBus<BasePacket>();
        ConfigureUnity();
        SetupServices();
        Serialization.LoadSerializers();
        _serverPacketListener = new ServerListener(ServerPackets);
    }

    void Update()
    {
        Networking?.Update();
    }

    private void OnApplicationQuit()
    {
        Networking?.Dispose();
    }

    public void SetupServices()
    {
        ServiceContainer.Register<IInputService, InputService>(CreateInputService());
        ServiceContainer.Register<IScreenService, ScreenService>(new ScreenService());
        ServiceContainer.Register<IAudioService, AudioService>(new AudioService());
        ServiceContainer.Register<INotificationService, NotificationService>(new NotificationService());
        ServiceContainer.Register<IAssetService, AssetService>(new AssetService());
    }

    InputService CreateInputService()
    {
        return gameObject?.AddComponent<InputService>();
    }
}