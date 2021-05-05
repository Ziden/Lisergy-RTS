using Assets.Code;
using Game.Events;
using UnityEngine;

public class MainBehaviour : MonoBehaviour
{
    public static Networking Networking { get; private set; }
    public static ClientPlayer Player { get; private set; }
    private static ServerListener GameListener { get; set; }

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
        new ServerEventSink();
        ConfigureUnity();
        Serialization.LoadSerializers();
        Networking = new Networking();
        GameListener = new ServerListener();
    }

    void Update()
    {
        Networking?.Update();
        Awaiter.Update();
        GameInput.Update();
    }

    private void OnApplicationQuit()
    {
        Networking?.Dispose();
    }
}
