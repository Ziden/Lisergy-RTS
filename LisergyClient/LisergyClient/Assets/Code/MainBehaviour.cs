using Assets.Code;
using Assets.Code.Login;
using Game;
using Game.Events;
using Game.Events.Bus;
using Game.Listeners;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainBehaviour : MonoBehaviour
{
    private static bool _globalMain = false;
    private bool _main = false;
    public static Networking Networking { get; private set; }
    public static ClientPlayer Player { get; set; }
    public static EventBus NetworkEvents { get; set; }
    public static ClientBlockchainGame Game { get; set; }

    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    void Awake()
    {
        if(!_globalMain)
        {
            Log.Debug("Starting Main Behaviour");
            Game = new ClientBlockchainGame();
            Networking = new Networking();
            NetworkEvents = new EventBus();

            NetworkEvents.RegisterListener(new GameListener());
            NetworkEvents.RegisterListener(new LoginPacketListener());
            NetworkEvents.RegisterListener(new DungeonPacketListener());

            ConfigureUnity();
            Serialization.LoadSerializers();
            _globalMain = true;
            _main = true;
        }
    }

    public static GameObject CreatePrefab(UnityEngine.Object prefab)
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
        global::Game.Log.Debug = Debug.Log;
        global::Game.Log.Error = Debug.LogError;
        global::Game.Log.Info = Debug.Log;
    }

    void Update()
    {
        if (!_main)
            return;

        Networking?.Update();
        Awaiter.Update();
        GameInput.Update();

        // For easier debugging
        if(Player == null && SceneManager.GetActiveScene().name != "Login")
            DebugAutoReauth();
    }

    void DebugAutoReauth()
    {
        if (Awaiter.IsCooldown("debugCon"))
            return;
       
        var login = PlayerPrefs.GetString("lastLogin");
        Log.Info($"Re authenticathing user {login}");
        var ev = new AuthPacket()
        {
            Login = login,
            Password = PlayerPrefs.GetString("lastPass")
        };
        Networking.Send(ev);
        Awaiter.SetCooldown("debugCon", TimeSpan.FromSeconds(5));
    }

    private void OnApplicationQuit()
    {
        Networking?.Dispose();
    }
}
