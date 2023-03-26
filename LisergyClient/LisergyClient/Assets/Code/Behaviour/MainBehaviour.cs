using Assets.Code;
using Game;
using Game.Events;
using Game.Events.Bus;
using UnityEngine;

public class MainBehaviour : MonoBehaviour
{
    public static Networking Networking { get; private set; }
    public static LocalPlayer LocalPlayer { get; private set; }
    public static EventBus<ServerPacket> ServerPackets { get; set; }

    private ServerListener _serverPacketListener;

    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        ClientEvents.OnPlayerLogin += OnPlayerLogin;
    }

    public static GameObject CreatePrefab(Object prefab)
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

    private void ConfigureUnity()
    {
        Application.runInBackground = true;
        Telepathy.Logger.Log = Debug.Log;
        Telepathy.Logger.LogWarning = Debug.LogWarning;
        Telepathy.Logger.LogError = Debug.LogError;
        Game.Log.Debug = Debug.Log;
        Game.Log.Error = Debug.LogError;
        Game.Log.Info = Debug.Log;
    }

    private void Awake()
    {
        Networking = new Networking();
        ServerPackets = new EventBus<ServerPacket>();
        ConfigureUnity();
        SetupServices();
        Serialization.LoadSerializers();
        _serverPacketListener = new ServerListener(ServerPackets);
    }

    void Update()
    {
        Networking?.Update();
        Awaiter.Update();
    }

    private void OnApplicationQuit()
    {
        Networking?.Dispose();
    }

    private void SetupServices()
    {
        var inputManager = CreateInputManager();
        Global.Register<IInputManager, InputManager>(inputManager);
    }


    InputManager CreateInputManager()
    {
        return gameObject.AddComponent<InputManager>();
    }
}