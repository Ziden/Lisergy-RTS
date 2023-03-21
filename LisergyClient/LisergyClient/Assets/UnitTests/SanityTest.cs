using System.Collections;
using UnityEngine.TestTools;
using Assets.UnitTests;
using NUnit.Framework;
using Assets.UnitTests.Stubs;
using Game.World;
using Game.Tile;
using UnityEngine;

namespace Tests
{
    public class TestScript
    {
        [UnityTest]
        public IEnumerator TestSanity() 
        {
            yield return UnityPlaytestUtils.LoadScene("Game");

            StubServerThread.SetupTestServer();

            var client = new SmokeClient();

            yield return client.LoginBehaviour.Login();
            yield return client.LoginBehaviour.JoinWorld();


            var party = MainBehaviour.Player.Parties[0];
            client.ClickTile(party.Tile);

            var otherTile = party.Tile.GetNeighbor(Direction.SOUTH);
            client.ClickTile(otherTile);
            yield return null;

            UIManager.ActionsUI.MoveButton();
            yield return new WaitForSeconds(0.2f);

            Assert.AreEqual(party.Tile, otherTile);

            Assert.IsTrue(true);
        }
    }
}
