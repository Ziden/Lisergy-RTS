using Assets.Code;
using Game;
using Game.Events;
using Game.Events.ServerEvents;
using UnityEngine;
using UnityEngine.UI;

public class LoginCanvas
{
    public GameObject GameObject;
    public InputField Login;
    public InputField Password;
    public Button LoginButton;

    public LoginCanvas()
    {
        GameObject = GameObject.Find("LoginCanvas");
        Login = GameObject.transform.GetRequiredChildComponent<InputField>("Login");
        Password = GameObject.transform.GetRequiredChildComponent<InputField>("Password");
        LoginButton = GameObject.transform.GetRequiredChildComponent<Button>("LoginButton");
        LoginButton.onClick.AddListener(Authenticate);
        EventSink.OnPlayerAuth += OnPlayerAuth;
    }

    public void OnPlayerAuth(AuthResultEvent ev)
    {
        if(ev.Success)
        {
            MainBehaviour.Player = new ClientPlayer()
            {
                ID = ev.PlayerID
            };
            Log.Info($"Auth suceeded, player logged in");
            MainBehaviour.Player.Send(new JoinWorldEvent());
            GameObject.SetActive(false);
        } else
        {
            Log.Error($"Auth failed");
        }
    }

    public void Authenticate()
    {
        Log.Debug("Sending Auth");
        var login = Login.text;
        var pass = Password.text;
        var ev = new AuthEvent()
        {
            Login = login,
            Password = pass
        };
        MainBehaviour.Networking.Send<AuthEvent>(ev);
    }
}
