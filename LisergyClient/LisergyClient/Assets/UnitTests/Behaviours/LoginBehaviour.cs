using Assets.Code;
using Assets.UnitTests.Stubs;
using Game.Network.ClientPackets;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.UnitTests.Behaviours
{
    public class LoginBehaviour
    {
        public IEnumerator Login()
        {
            yield return Wait.Until(() => UIManager.LoginCanvas != null);
            var loginScreen = UIManager.LoginCanvas;
            loginScreen.Login.text = "TEST";
            loginScreen.Password.text = "TESTPASS";
            loginScreen.LoginButton.onClick.Invoke();
            Debug.Log("Clicking Login");
            yield return Wait.Until(() => GameView.Initialized);
        }

        public IEnumerator JoinWorld()
        {
            MainBehaviour.Networking.Send(new JoinWorldPacket());
            yield return new WaitUntil(() => MainBehaviour.Player.Parties != null && MainBehaviour.Player.Parties[0] != null);
        }

    }
}
