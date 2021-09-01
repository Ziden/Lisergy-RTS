using Assets.Code;
using Game;
using Game.Events;
using Game.Events.Bus;
using Game.Events.ServerEvents;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginUI : MonoBehaviour, IEventListener
{
    public GameObject GameObject { get => this.gameObject; }
    public InputField Login;
    public InputField Password;
    public Button LoginButton;

    void Start()
    {
        Login = GameObject.transform.GetRequiredChildComponent<InputField>("Login");
        Password = GameObject.transform.GetRequiredChildComponent<InputField>("Password");
        LoginButton = GameObject.transform.GetRequiredChildComponent<Button>("LoginButton");
        LoginButton.onClick.AddListener(Authenticate);
        MainBehaviour.NetworkEvents.RegisterListener(this);
    }

    [EventMethod]
    public void OnPlayerAuth(AuthResultPacket ev)
    {
        if (ev.Success)
            GameObject.SetActive(false);
    }

    public void Authenticate()
    {
        Log.Debug("Sending Auth");
        var login = Login.text;
        var pass = Password.text;
        var ev = new AuthPacket()
        {
            Login = login,
            Password = pass
        };
        MainBehaviour.Networking.Send<AuthPacket>(ev);
        PlayerPrefs.SetString("lastLogin", login);
        PlayerPrefs.SetString("lastPass", pass);

    }
}
