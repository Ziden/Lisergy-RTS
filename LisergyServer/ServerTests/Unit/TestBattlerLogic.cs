using Game;
using Game.Battler;
using Game.DataTypes;
using Game.ECS;
using Game.Network;
using Game.Party;
using Game.Player;
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
            var clientEntity = new PartyEntity(new Gaia());

            var clientComponent = clientEntity.Components.Add<BattleGroupComponent>();
            var serverComponent = new BattleGroupComponent();
            serverComponent.BattleID = GameId.Generate();

            ComponentSynchronizer.SyncComponents(clientEntity, new List<IComponent>() { serverComponent });

            Assert.AreEqual(clientComponent.BattleID, serverComponent.BattleID);
        }

        [Test]
        public void TestLogicTriggeringEvents()
        {
            var clientEntity = new PartyEntity(new Gaia());
            var triggered = false;

            clientEntity.BattleGroupLogic.OnBattleIdChanged += (_, _) => triggered = true;

            var clientComponent = clientEntity.Components.Add<BattleGroupComponent>();
            var serverComponent = new BattleGroupComponent();
            serverComponent.BattleID = GameId.Generate();

            ComponentSynchronizer.SyncComponents(clientEntity, new List<IComponent>() { serverComponent });

            Assert.IsTrue(triggered);
        }
    }
}
