using Assets.Code;
using Assets.Code.Assets.Code.UIScreens;
using Assets.Code.Assets.Code.UIScreens.Base;
using Assets.UnitTests.Stubs;
using Game.Network.ClientPackets;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.UnitTests.Behaviours
{
    public class LoginBehaviour
    {
        private IScreenService _screens;

        public LoginBehaviour()
        {
            _screens = ServiceContainer.Resolve<IScreenService>();
        }

        public IEnumerator Login()
        {
            yield return Wait.Until(() => _screens.IsOpen<LoginScreen>(), "Game scene not properly initialized");
            var loginScreen = _screens.Get<LoginScreen>();
            loginScreen.Login.SetValueWithoutNotify("Test Player "+Guid.NewGuid().ToString());
            loginScreen.Password.SetValueWithoutNotify("Test");
            using (var e = new NavigationSubmitEvent() { target = loginScreen.Submit })
                loginScreen.Submit.SendEvent(e);
            Debug.Log("Clicking Login");
            yield return Wait.Until(() => GameView.Initialized, "Game view not initialized after logging in");
        }

        public IEnumerator JoinWorld()
        {
            MainBehaviour.Networking.Send(new JoinWorldPacket());
            yield return Wait.Until(() => GameView.World != null , "Did not receive world tiles after joining world");
        }

    }
}
