using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;
using Assets.UnitTests;
using Game.World;
using Assets.Code.World;
using NUnit.Framework;

namespace Tests
{
    public class TestScript
    {
        [UnityTest]
        public IEnumerator TestSanity() // needs a running server
        {
            var game = MonoBehaviour.Instantiate(Resources.Load<GameObject>("prefabs/Game"));
            yield return null;

            var client = new SmokeClient();

            client.Login();
            yield return null;

            yield return new WaitForSeconds(1f);

            var party = MainBehaviour.Player.Parties[0];
            client.ClickTile(party.Tile);
            yield return null;

            var otherTile = party.Tile.GetNeighbor(Direction.SOUTH);
            client.ClickTile(otherTile);
            yield return null;

            UIManager.ActionsUI.MoveButton();
            yield return new WaitForSeconds(0.2f);

            Assert.AreEqual(party.Tile, otherTile);
        }
    }
}
