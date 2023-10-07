using Game;
using Game.ECS;
using Game.Events.GameEvents;
using Game.Events.ServerEvents;
using Game.Network;
using Game.Systems.Dungeon;
using Game.Systems.FogOfWar;
using Game.Systems.Party;
using Game.Systems.Player;
using NUnit.Framework;
using ServerTests;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;

namespace Tests
{
    public unsafe class TestECS
    {
        [StructLayout(LayoutKind.Sequential)]
        [SyncedComponent]
        internal struct SimpleComponent : IComponent
        {
            public int PublicField;
            public int Property { get; set; }
        }

        [StructLayout(LayoutKind.Sequential)]
        [SyncedComponent(OnlyMine = false)]
        internal struct PublicSyncComponent : IComponent
        {
            public int Property { get; set; }
        }

        [StructLayout(LayoutKind.Sequential)]
        [SyncedComponent(OnlyMine=true)]
        internal struct SelfSyncComponent : IComponent
        {
            public int Property { get; set; }
        }

        [Test]
        public void TestRegistrationByType()
        {
            var components = new ComponentSet(new DungeonEntity(null));
            IComponent toAdd = new EntityVisionComponent();
            components.Add(typeof(EntityVisionComponent), toAdd);

            Assert.AreEqual(toAdd, components.Get<EntityVisionComponent>());
        }

        [Test]
        public void TestGenericAdd()
        {
            var components = new ComponentSet(new DungeonEntity(null));
            IComponent toAdd = new EntityVisionComponent();
            components.Add(toAdd);

            Assert.AreEqual(toAdd, components.Get<EntityVisionComponent>());
        }

        [Test]
        public void TestSyncOnlyMine()
        {
            var player = new TestServerPlayer(new TestGame());
            var clientEntity = new PartyEntity(player.Game, player);
            var selfComponent = clientEntity.Components.Add(new SelfSyncComponent());
            var publicComponent = clientEntity.Components.Add(new PublicSyncComponent());

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
        public void TestComponentWontModifyNoSave()
        {
            var player = new TestServerPlayer(new TestGame());
            var clientEntity = new PartyEntity(player.Game, player);
            clientEntity.Components.Add(new SelfSyncComponent()
            {
                Property = 666
            });

            var component1 = clientEntity.Get<SelfSyncComponent>();

            component1.Property = 123;

            var component2 = clientEntity.Get<SelfSyncComponent>();

            Assert.AreEqual(123, component1.Property);
            Assert.AreEqual(666, component2.Property);
        }

        [Test]
        public void TestComponentModifiesAfterSave()
        {
            var player = new TestServerPlayer(new TestGame());
            var clientEntity = new PartyEntity(player.Game, player);
            clientEntity.Components.Add(new SelfSyncComponent()
            {
                Property = 666
            });

            var component1 = clientEntity.Get<SelfSyncComponent>();

            component1.Property = 123;
            clientEntity.Save(component1);

            var component2 = clientEntity.Get<SelfSyncComponent>();

            Assert.AreEqual(123, component1.Property);
            Assert.AreEqual(123, component2.Property);
        }

        [StructLayout(LayoutKind.Sequential)]
        [SyncedComponent(OnlyMine = true)]
        internal struct SelfSyncComponentMagicTracker : IComponent
        {
            private int _v;
            public int Property { get => _v; set => _v = value; }
        }

        [Test]
        public void TestPointerRefs()
        {
            var player = new TestServerPlayer(new TestGame());
            var clientEntity = new PartyEntity(player.Game, player);
            clientEntity.Components.Add(new SelfSyncComponent()
            {
                Property = 666
            });
        }
    }
}