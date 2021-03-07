/*
using System.Collections;
using Assets.Code;
using Game;
using Game.Events;
using Game.Events.ServerEvents;
using GameData;
using LisergyServer.Core;
using NUnit.Framework;
using UnityEngine.TestTools;
*/
using UnityEngine;
namespace Tests
{
    public class Wololo : MonoBehaviour
    {
        /*
        private GameObject _main;
        private StrategyGame _server;

        public void CreateShit()
        {
            _main = new GameObject("MainBehaviour");
            new GameObject("LoginCanvas");

            _main.AddComponent<MainBehaviour>();

            var cfg = new GameConfiguration();
            var specs = new GameSpec();
            var world = new GameWorld();
            var game = new StrategyGame(cfg, specs, world);
            Serialization.LoadSerializers();
            world.CreateWorld(4);
            world.ChunkMap.SetFlag(0, 0, ChunkFlag.NEWBIE_CHUNK);
        }

        public void SendToServer(PlayerEntity sender, ClientEvent ev)
        {
            NetworkEvents.SERVER = false;
            var bytes = Serialization.FromEvent(ev);
            EventEmitter.CallEventFromBytes(sender, bytes);
            NetworkEvents.SERVER = true;
        }

        // A Test behaves as an ordinary method
        [Test]
        public void WololoSimplePasses()
        {
            // Use the Assert class to test conditions
            CreateShit();

            var player = new ClientPlayer();
            var loginCanvas = new LoginCanvas();
            loginCanvas.OnPlayerAuth(new AuthResultEvent() {
                FromNetwork = true,
                PlayerID = player.UserID,
                Success = true,
                Sender = player
            });

            var joinEvent = new JoinWorldEvent();
            

            Assert.IsTrue(false);
        }

        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator WololoWithEnumeratorPasses()
        {
            // Use the Assert class to test conditions.
            // Use yield to skip a frame.
            yield return null;
        }
        */
    }
}
