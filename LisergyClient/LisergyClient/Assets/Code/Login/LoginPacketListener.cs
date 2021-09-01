using Game;
using Game.Events;
using Game.Events.Bus;
using Game.Events.ServerEvents;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace Assets.Code.Login
{
    public class LoginPacketListener: IEventListener
    {
        [EventMethod]
        public void ReceivePlayer(PlayerPacket ev)
        {
            MainBehaviour.Player = new ClientPlayer();
            MainBehaviour.Player.Units = new HashSet<Unit>(ev.Units);
            Log.Debug($"Received {ev.Units.Length} units");
            if(SceneManager.GetActiveScene().name == "Login")
                SceneManager.LoadScene("Town", LoadSceneMode.Single);
        }

        [EventMethod]
        public void OnPlayerAuth(AuthResultPacket ev)
        {
            Log.Debug($"Received player auth. Suceess ?{ev.Success}");
            if (ev.Success)
            {
                var player = new ClientPlayer()
                {
                    UserID = ev.PlayerID
                };
                MainBehaviour.Networking.Send(new JoinWorldPacket());
                ClientEvents.PlayerLogin(player);
            }
            else
            {
                Log.Error($"Auth failed");
            }
        }
    }
}
