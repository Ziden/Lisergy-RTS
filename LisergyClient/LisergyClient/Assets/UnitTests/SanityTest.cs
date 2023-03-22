using System.Collections;
using UnityEngine.TestTools;
using Assets.UnitTests;
using NUnit.Framework;
using Assets.UnitTests.Stubs;
using Game.World;
using Game.Tile;
using UnityEngine;
using Assets.Code;
using System.Linq;
using Game.FogOfWar;
using Game;
using Game.Dungeon;
using Assets.Code.World;
using Game.Party;

namespace Tests
{
    public class TestScript
    {
        private SmokeClient _client;


        [UnityTest]
        public IEnumerator TestLogin() 
        {
            yield return UnityPlaytestUtils.LoadScene("Game");
            _client = new SmokeClient();

            #region Login
            yield return _client.LoginBehaviour.Login();
            yield return _client.LoginBehaviour.JoinWorld();
            #endregion

            #region Move Party
            yield return _client.PartyBehaviour.SelectParty(0);
            var party = UIManager.PartyUI.SelectedParty;
            var east = party.Tile.GetNeighbor(Direction.NORTH);
            yield return _client.PartyBehaviour.MoveWithSelected(Direction.NORTH);
            Assert.That(party.Tile == east, "Party did not move");
            #endregion

            #region Building Checks
            var initialBuilding = MainBehaviour.LocalPlayer.Buildings.FirstOrDefault(b => b.SpecID == StrategyGame.Specs.InitialBuilding);
            Assert.That(initialBuilding != null, "Did not receive initial building");
            #endregion

            #region Line of Sight
            var buildingLos = MainBehaviour.LocalPlayer.Buildings.First().Components.Get<EntityVisionComponent>().LineOfSight;
            var partyLos = (int)party.Components.Get<EntityVisionComponent>().LineOfSight;
            Assert.That(partyLos > 0, "Party line of sight cannot be zero");
            Assert.That(buildingLos > 0, "Building line of sight cannot be zero");
          
            var lastLosTile = party.Tile.GetNeighborInRange(partyLos, Direction.NORTH);
            var unseenTile = lastLosTile.GetNeighbor(Direction.NORTH);

            Assert.IsTrue(lastLosTile.EntitiesViewing.Any(e => e == party));
            Assert.IsFalse(unseenTile.EntitiesViewing.Any(e => e == party));

            yield return _client.PartyBehaviour.MoveWithSelected(Direction.NORTH);

            Assert.IsTrue(lastLosTile.EntitiesViewing.Any(e => e == party));
            Assert.IsTrue(unseenTile.EntitiesViewing.Any(e => e == party));

            yield return _client.PartyBehaviour.MoveWithSelected(Direction.NORTH, partyLos);

            lastLosTile = party.Tile.GetNeighborInRange(partyLos, Direction.SOUTH);
            unseenTile = lastLosTile.GetNeighbor(Direction.SOUTH);

            Assert.IsTrue(lastLosTile.EntitiesViewing.Any(e => e == party));
            Assert.IsFalse(unseenTile.EntitiesViewing.Any(e => e == party));
            #endregion

            #region Dungeon    
            var dungeon = _client.FindFirst<DungeonEntity>();

            yield return _client.PartyBehaviour.MoveAttackWithSelected(dungeon.Tile);
            yield return Wait.Until(() => EntityEffects<PartyEntity>.HasEffects(party), "battle animation did not play");
            yield return Wait.Until(() => !EntityEffects<PartyEntity>.HasEffects(party), "battle animation did not stop");

            var dungeonUnit = dungeon.BattleGroupLogic.GetUnits().First();
            Assert.That(dungeonUnit.HP < dungeonUnit.MaxHP, "Client did not receive dungeon unit updates");
            Assert.That(party.Tile == initialBuilding.Tile, "Client party did not move back after being destroyed");
            #endregion
        }
    }
}
