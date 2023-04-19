using Assets.Code.Assets.Code.UIScreens;
using Assets.Code.Assets.Code.UIScreens.Base;
using Game;
using Game.Events.Bus;
using Game.Events.ServerEvents;
using Game.Network.ClientPackets;
using GameAssets;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace Assets.Code
{
    public class LoginScreen : UITKScreen, IEventListener
    {
        public TextField Login;
        public TextField Password;
        public Button Submit;

        public override UIScreen ScreenAsset => UIScreen.LoginScreen;

        public override void OnBeforeOpen()
        {
            SceneManager.LoadSceneAsync("Login", LoadSceneMode.Additive);
        }

        public override void OnClose()
        {
            SceneManager.UnloadSceneAsync("Login");
        }

        public override void OnLoaded(VisualElement root)
        {
            var service = ServiceContainer.Resolve<IScreenService>();
            Login = root.Q<TextField>("Login");
            Password = root.Q<TextField>("Password");
            Submit = root.Q<Button>("LoginButton");
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
            }
            else
            {
                Log.Error($"Auth failed");
            }
        }

    }
}
