using Assets.Code;
using Game.Events;
using Game.Events.Bus;
using UnityEngine;

public class MainBehaviour : MonoBehaviour
{
    public static Networking Networking { get; private set; }
    public static ClientPlayer Player { get; private set; }
    public static EventBus NetworkEvents { get; set; }

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

    public void OnPlayerLogin(ClientPlayer player)
    {
        MainBehaviour.Player = player;
    }

    private void ConfigureUnity()
    {
        Application.runInBackground = true;
        Telepathy.Logger.Log = Debug.Log;
        Telepathy.Logger.LogWarning = Debug.LogWarning;
        Telepathy.Logger.LogError = Debug.LogError;
        global::Game.Log.Debug = Debug.Log;
        global::Game.Log.Error = Debug.LogError;
        global::Game.Log.Info = Debug.Log;
    }

    private void Awake()
    {
        Networking = new Networking();
        NetworkEvents = new EventBus();
        ConfigureUnity();
        SetupServices();
        Serialization.LoadSerializers();
        _serverPacketListener = new ServerListener(NetworkEvents);
        SetupCamera();
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


    private void SetupCamera()
    {
        var camera = Resources.Load("prefabs/Camera");
        Instantiate(camera, gameObject.transform);
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