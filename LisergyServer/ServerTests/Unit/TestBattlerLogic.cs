using Game.DataTypes;
using Game.ECS;
using Game.Network;
using Game.Systems.Battler;
using Game.Systems.Party;
using Game.Systems.Player;
using NUnit.Framework;
using ServerTests;
using System.Collections.Generic;

namespace Tests
{
    public class TestBattlerLogic
    {
        private TestGame Game;

        [SetUp]
        public void Setup()
        {
            Game = new TestGame();
        }

        [Test]
        public void TestBattleComponentLogicSync()
        {
            var clientEntity = new PartyEntity(null);

            var clientComponent = clientEntity.Components.Add<BattleGroupComponent>();
            var serverComponent = new BattleGroupComponent();
            serverComponent.BattleID = GameId.Generate();

            ComponentSynchronizer.SyncComponents(clientEntity, new List<IComponent>() { serverComponent });

            Assert.AreEqual(clientComponent.BattleID, serverComponent.BattleID);
        }
    }
}
