using Assets.Code;
using Assets.UnitTests.Stubs;
using Game.Network.ClientPackets;
using System;
using System.Collections;
using UnityEngine;

namespace Assets.UnitTests.Behaviours
{
    public class LoginBehaviour
    {
        public IEnumerator Login()
        {
            yield return Wait.Until(() => UIManager.LoginCanvas != null, "Game scene not properly initialized");
            var loginScreen = UIManager.LoginCanvas;
            loginScreen.Login.text = "Test Player "+Guid.NewGuid().ToString();
            loginScreen.Password.text = "Test";
            loginScreen.LoginButton.onClick.Invoke();
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
