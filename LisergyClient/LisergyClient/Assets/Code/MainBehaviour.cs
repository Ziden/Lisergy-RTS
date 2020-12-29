using Assets.Code;
using Game.Events;
using UnityEngine;

public class MainBehaviour : MonoBehaviour
{
    public static Networking Networking;
    public static ClientPlayer Player;
    public static ClientStrategyGame StrategyGame;
    private static ServerListener GameListener;

    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    public static GameObject CreatePrefab(Object prefab)
    {
        return Instantiate(prefab) as GameObject;
    }

    public static void DestroyObject(GameObject obj)
    {
        Destroy(obj);
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
        EventSink.SERVER = false; 
        ConfigureUnity();
        Serialization.LoadSerializers();
        Networking = new Networking();
        GameListener = new ServerListener();
    }

    void Update()
    {
        Networking?.Update();
        Awaiter.Update();
        StrategyGame?.Update();
    }

    private void OnApplicationQuit()
    {
        Networking?.Dispose();
    }
}
