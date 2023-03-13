using Assets.Code;
using Game;
using Game.Events;
using Game.Events.Bus;
using Game.Events.ServerEvents;
using UnityEngine;
using UnityEngine.UI;

public class LoginCanvas : IEventListener
{
    public GameObject GameObject;
    public InputField Login;
    public InputField Password;
    public Button LoginButton;

    public LoginCanvas(GameObject loginCanvas)
    {
        GameObject = loginCanvas;
        Login = GameObject.transform.GetRequiredChildComponent<InputField>("Login");
        Password = GameObject.transform.GetRequiredChildComponent<InputField>("Password");
        LoginButton = GameObject.transform.GetRequiredChildComponent<Button>("LoginButton");
        LoginButton.onClick.AddListener(Authenticate);
        MainBehaviour.NetworkEvents.Register<AuthResultPacket>(this, OnPlayerAuth);
    }

    [EventMethod]
    public void OnPlayerAuth(AuthResultPacket ev)
    {
        if(ev.Success)
        {
            var player = new ClientPlayer()
            {
                UserID = ev.PlayerID
            };
            MainBehaviour.Networking.Send(new JoinWorldPacket());
            ClientEvents.PlayerLogin(player);
            GameObject.SetActive(false);
            UIManager.Notifications.ShowNotification("Welcome to Lisergy");
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
        var ev = new AuthPacket()
        {
            Login = login,
            Password = pass
        };
        MainBehaviour.Networking.Send<AuthPacket>(ev);
    }
}
