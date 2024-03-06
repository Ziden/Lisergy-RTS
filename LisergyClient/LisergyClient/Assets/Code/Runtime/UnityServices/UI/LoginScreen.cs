using Assets.Code.Assets.Code.UIScreens;
using Assets.Code.Assets.Code.UIScreens.Base;
using Game;
using Game.Engine.Events.Bus;
using Game.Events.ServerEvents;
using Game.Network.ClientPackets;
using GameAssets;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace Assets.Code
{
    public class LoginScreen : GameUi, IEventListener
    {
        public TextField Login;
        public TextField Password;
        public Button Submit;

        public override UIScreen UiAsset => UIScreen.LoginScreen;

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
            var service = UnityServicesContainer.Resolve<IUiService>();
            Login = root.Q<TextField>("Login");
            Password = root.Q<TextField>("Password");
            Submit = root.Q<Button>("LoginButton");
            Submit.clicked += Authenticate;
        }

        public void Authenticate()
        {
            GameClient.Modules.Account.SendAuthenticationPacket(Login.text, Password.text);
        }
    }
}
