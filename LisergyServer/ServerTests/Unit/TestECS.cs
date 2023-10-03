using Game;
using Game.ECS;
using Game.Events.GameEvents;
using Game.Events.ServerEvents;
using Game.Network;
using Game.Systems.Dungeon;
using Game.Systems.FogOfWar;
using Game.Systems.Player;
using NUnit.Framework;
using ServerTests;
using System.Collections.Generic;

namespace Tests
{
    public unsafe class TestECS
    {
        [SyncedComponent]
        internal class SimpleComponent : IComponent
        {
            public int PublicField;
            public int Property { get; set; }
        }

        [SyncedComponent(OnlyMine = false)]
        internal class PublicSyncComponent : IComponent
        {
            public int Property { get; set; }
        }

        [SyncedComponent(OnlyMine=true)]
        internal class SelfSyncComponent : IComponent
        {
            public int Property { get; set; }
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

        [Test]
        public void TestComponentSync()
        {
            var clientEntity = new BaseEntity(null);

            SimpleComponent fromServer = new SimpleComponent() { PublicField = 5, Property = 4 };
            SimpleComponent inClient = new SimpleComponent();

            clientEntity.Components.Add(inClient);

            ComponentSynchronizer.SyncComponents(clientEntity, new List<IComponent>() { fromServer });

            Assert.AreEqual(fromServer.Property, inClient.Property, "Property should be copied");
            Assert.AreEqual(fromServer.PublicField, inClient.PublicField, "Public field should be copied");
        }

        [Test]
        public void TestSyncOnlyMine()
        {
            var player = new TestServerPlayer(null);
            var clientEntity = new BaseEntity(player);
            var selfComponent = clientEntity.Components.Add<SelfSyncComponent>();
            var publicComponent = clientEntity.Components.Add<PublicSyncComponent>();

            var selfPacket = clientEntity.GetUpdatePacket(player) as EntityUpdatePacket;
            var publicPacket = clientEntity.GetUpdatePacket(null) as EntityUpdatePacket;

            Assert.IsTrue(selfPacket.SyncedComponents.Contains(selfComponent));
            Assert.IsTrue(selfPacket.SyncedComponents.Contains(publicComponent));
            Assert.IsTrue(!publicPacket.SyncedComponents.Contains(selfComponent));
            Assert.IsTrue(publicPacket.SyncedComponents.Contains(publicComponent));

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