using Assets.Code;
using Game.Events;
using UnityEngine;

public class MainBehaviour : MonoBehaviour
{
    public static Networking Networking;
    public static ClientPlayer Player;

    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
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
        Networking = new Networking();
    }

    void Update()
    {
        Networking.Update();
    }

    private void OnApplicationQuit()
    {
        Networking.Dispose();
    }
}
