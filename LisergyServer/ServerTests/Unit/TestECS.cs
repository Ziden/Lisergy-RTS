using Game;
using Game.Battler;
using Game.Dungeon;
using Game.ECS;
using Game.Events.GameEvents;
using Game.FogOfWar;
using Game.Packets;
using Game.Player;
using NUnit.Framework;
using System.Collections.Generic;

namespace Tests
{
    public unsafe class TestECS
    {

        [SetUp]
        public void Setup()
        {

        }

        [Test]
        public void TestRegistrationByType()
        {
            var components = new ComponentSet(new DungeonEntity());
            IComponent toAdd = new EntityVisionComponent();
            components.Add(typeof(EntityVisionComponent), toAdd);

            Assert.AreEqual(toAdd, components.Get<EntityVisionComponent>());
        }

        [Test]
        public void TestGenericAdd()
        {
            var components = new ComponentSet(new DungeonEntity());
            IComponent toAdd = new EntityVisionComponent();
            components.Add(toAdd);

            Assert.AreEqual(toAdd, components.Get<EntityVisionComponent>());
        }

        internal class SimpleComponent : IComponent
        {
            public int PublicField;
            public int Property { get; set; }
        }


        [Test]
        public void TestComponentSync()
        {
            var clientEntity = new WorldEntity(new Gaia());

            SimpleComponent fromServer = new SimpleComponent() { PublicField = 5, Property = 4 };
            SimpleComponent inClient = new SimpleComponent();

            clientEntity.Components.Add(inClient);

            EntitySynchronizer.SyncComponents(clientEntity, new List<IComponent>() { fromServer });

            Assert.AreEqual(fromServer.Property, inClient.Property, "Property should be copied");
            Assert.AreEqual(fromServer.PublicField, inClient.PublicField, "Public field should be copied");
        }

        [Test]
        public void TestBattleComponentLogicSync()
        {
            var clientEntity = new WorldEntity(new Gaia());
            var logic = new BattleGroupComponentLogic(clientEntity);

            SimpleComponent fromServer = new SimpleComponent() { PublicField = 5, Property = 4 };
            SimpleComponent inClient = new SimpleComponent();

            clientEntity.Components.Add(inClient);

            EntitySynchronizer.SyncComponents(clientEntity, new List<IComponent>() { fromServer });

            Assert.AreEqual(fromServer.Property, inClient.Property, "Property should be copied");
            Assert.AreEqual(fromServer.PublicField, inClient.PublicField, "Public field should be copied");
        }

        public class TestView : IComponent
        {
            public static EntityMoveOutEvent called = null;

            public static void Callback(TestView view, EntityMoveOutEvent ev)
            {
                TestView.called = ev;
            }
        }

        [Test]
        public void TestViewEvents()
        {
            /*
            tile1.Components.Add(new TestView());

            
            tile1._components.RegisterExternalComponentEvents<TestView, EntityMoveOutEvent>(TestView.Callback);

            tile1.Components.CallEvent(new EntityMoveOutEvent() { Entity = new DungeonEntity() });

            Assert.IsTrue(TestView.called.Entity is DungeonEntity);
            */

        }
    }
}