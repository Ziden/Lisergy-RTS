using Assets.Code.Assets.Code.UIScreens;
using Game;
using Game.Events.Bus;
using Game.Events.ServerEvents;
using Game.Network.ClientPackets;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Code
{
    public class LoginScreen : MonoBehaviour, IEventListener
    {
        public VisualElement Root;

        public TextField Login;
        public TextField Password;
        public Button Submit;

        void Start()
        {
            var service = ServiceContainer.Resolve<IScreenService>();
            Root = service.LoadAndAttach(this, "LoginScreen");
            Login = Root.Q<TextField>("Login");
            Password = Root.Q<TextField>("Password");
            Submit = Root.Q<Button>("LoginButton");
            Submit.clicked += Authenticate;
            MainBehaviour.ServerPackets.Register<AuthResultPacket>(this, OnPlayerAuth);
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
            MainBehaviour.Networking.Send(ev);
        }

        [EventMethod]
        public void OnPlayerAuth(AuthResultPacket ev)
        {
            if (ev.Success)
            {
                var player = new LocalPlayer(ev.PlayerID);
                MainBehaviour.Networking.Send(new JoinWorldPacket());
                ClientEvents.PlayerLogin(player);
                ServiceContainer.Resolve<IScreenService>().Close<LoginScreen>();
                UIManager.Notifications.ShowNotification("Welcome to Lisergy");
            }
            else
            {
                Log.Error($"Auth failed");
            }
        }

    }
}
