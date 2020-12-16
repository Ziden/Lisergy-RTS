using Assets.Code;
using Assets.Code.World;
using Game.Events;
using UnityEngine;

public class MainBehaviour : MonoBehaviour
{
    public static Networking Networking;
    public static ClientPlayer Player;
    private static GameListener GameListener;

    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    public static GameObject CreatePrefab(Object prefab)
    {
        return Instantiate(prefab) as GameObject;
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
        ConfigureUnity();
        Serialization.LoadSerializers();
        Networking = new Networking();
        GameListener = new GameListener();
    }

    void Update()
    {
        Networking.Update();
        Awaiter.Update();
    }

    private void OnApplicationQuit()
    {
        Networking.Dispose();
    }
}
