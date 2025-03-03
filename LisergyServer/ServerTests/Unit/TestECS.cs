using Game.Engine;
using Game.Engine.DataTypes;
using Game.Engine.ECLS;
using Game.Engine.Events;
using Game.Entities;
using Game.Events.ServerEvents;
using Game.Systems.Map;
using NUnit.Framework;
using ServerTests;
using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace UnitTests
{
    public unsafe class TestECS
    {
        [Serializable]
        [StructLayout(LayoutKind.Sequential)]
        [SyncedComponent]
        internal class ClassComponent : IComponent
        {
            public int PublicField;
            public int Property { get; set; }
        }

        [Serializable]
        [StructLayout(LayoutKind.Sequential)]
        [SyncedComponent]
        internal struct SimpleComponent : IComponent
        {
            public int PublicField;
            public int Property { get; set; }
        }

        [Serializable]
        [StructLayout(LayoutKind.Sequential)]
        [SyncedComponent(OnlyMine = false)]
        internal struct PublicSyncComponent : IComponent
        {
            public int Property { get; set; }
        }

        [Serializable]
        [StructLayout(LayoutKind.Sequential)]
        [SyncedComponent(OnlyMine = true)]
        internal struct SelfSyncComponent : IComponent
        {
            public int Property { get; set; }
        }

        [SetUp]
        public void Setup()
        {
            Serialization.LoadSerializers(typeof(SelfSyncComponent), typeof(PublicSyncComponent), typeof(SimpleComponent));
        }

        [Test]
        public void TestDeltaTrackedOnAdd()
        {
            var player = new TestServerPlayer(new TestGame());
            var clientEntity = player.Game.Entities.CreateEntity(EntityType.Party, player.EntityId);

            clientEntity.Components.Add<SelfSyncComponent>();

            var deltas = clientEntity.Components.GetComponentDeltas(player.EntityId);

            Assert.IsTrue(deltas.updated.Contains(clientEntity.Components.Get<SelfSyncComponent>()));

            clientEntity.Logic.DeltaCompression.GetUpdatePacket(player.EntityId);

            var selfPacket = clientEntity.Logic.DeltaCompression.GetUpdatePacket(player.EntityId) as EntityUpdatePacket;

            Assert.IsTrue(selfPacket.SyncedComponents.Contains(clientEntity.Components.Get<SelfSyncComponent>()));
        }

        [Test]
        public void TestClassBeingTrackedOnadd()
        {
            var player = new TestServerPlayer(new TestGame());
            var clientEntity = player.Game.Entities.CreateEntity(EntityType.Party, player.EntityId);

            clientEntity.Components.Add<ClassComponent>();

            var deltas = clientEntity.Components.GetComponentDeltas(player.EntityId);

            Assert.IsTrue(deltas.updated.Any(c => c.GetType() == typeof(ClassComponent)));

            clientEntity.Logic.DeltaCompression.GetUpdatePacket(player.EntityId);

            var selfPacket = clientEntity.Logic.DeltaCompression.GetUpdatePacket(player.EntityId) as EntityUpdatePacket;

            Assert.IsTrue(selfPacket.SyncedComponents.Any(p => p.GetType() == typeof(ClassComponent)));
        }

        [Test]
        public void TestClassReferences()
        {
            var player = new TestServerPlayer(new TestGame());
            var clientEntity = player.Game.Entities.CreateEntity(EntityType.Party, player.EntityId);
            clientEntity.Components.Add<ClassComponent>();

            var readCopy = clientEntity.Components.TryGet<ClassComponent>(out var c);

            c.Property = 123;

            clientEntity.Save(c);

            var deltas = clientEntity.Components.GetComponentDeltas();

            var asd = 123;
        }

        [Test]
        public void TestSyncOnlyMine()
        {
            var player = new TestServerPlayer(new TestGame());
            var clientEntity = player.Game.Entities.CreateEntity(EntityType.Party, player.EntityId);
            clientEntity.Components.Add<SelfSyncComponent>();

            var selfComponent = clientEntity.Components.Get<SelfSyncComponent>();
            clientEntity.Components.Add<PublicSyncComponent>();
            var publicComponent = clientEntity.Get<PublicSyncComponent>();

            var selfPacket = clientEntity.Logic.DeltaCompression.GetUpdatePacket(player.EntityId) as EntityUpdatePacket;

            Assert.IsTrue(selfPacket.SyncedComponents.Contains(selfComponent));
            Assert.IsTrue(selfPacket.SyncedComponents.Contains(publicComponent));
        }

        [Test]
        public void TestSyncOnlyPublic()
        {
            var player = new TestServerPlayer(new TestGame());
            var clientEntity = player.Game.Entities.CreateEntity(EntityType.Party, player.EntityId);
            clientEntity.Components.Add<SelfSyncComponent>();

            var selfComponent = clientEntity.Components.Get<SelfSyncComponent>();
            clientEntity.Components.Add<PublicSyncComponent>();
            var publicComponent = clientEntity.Get<PublicSyncComponent>();

            var publicPacket = clientEntity.Logic.DeltaCompression.GetUpdatePacket(default) as EntityUpdatePacket;

            Assert.IsTrue(!publicPacket.SyncedComponents.Contains(selfComponent));
            Assert.IsTrue(publicPacket.SyncedComponents.Contains(publicComponent));
        }

        public class TestView : IComponent
        {
            public static ComponentUpdateEvent<MapPlacementComponent> called = default;

            public static void Callback(TestView view, ComponentUpdateEvent<MapPlacementComponent> ev)
            {
                TestView.called = ev;
            }
        }

        [Test]
        public void TestSimpleModify()
        {
            var player = new TestServerPlayer(new TestGame());
            var clientEntity = new BaseEntity(GameId.Generate(), player.Game, EntityType.Party);
            clientEntity.Components.Add<SelfSyncComponent>();
            var component1 = clientEntity.Get<SelfSyncComponent>();
            component1.Property = 666;
            clientEntity.Save(component1);

            var component2 = clientEntity.Get<SelfSyncComponent>();

            Assert.AreEqual(666, component2.Property);
        }

        [Test]
        public void TestComponentNoModifyIfNoSave()
        {
            var player = new TestServerPlayer(new TestGame());
            var clientEntity = new BaseEntity(GameId.Generate(), player.Game, EntityType.Party);
            clientEntity.Components.Add<SelfSyncComponent>();
            var component1 = clientEntity.Get<SelfSyncComponent>();
            component1.Property = 666;
            clientEntity.Save(component1);

            var component2 = clientEntity.Get<SelfSyncComponent>();
            component2.Property = 123;

            var component3 = clientEntity.Get<SelfSyncComponent>();


            Assert.AreEqual(666, component3.Property);
            Assert.AreEqual(123, component2.Property);
        }

        [Test]
        public void TestComponentModifiesAfterSave()
        {
            var player = new TestServerPlayer(new TestGame());
            var clientEntity = new BaseEntity(GameId.Generate(), player.Game, EntityType.Party);
            clientEntity.Components.Add<SelfSyncComponent>();
            var component1 = clientEntity.Get<SelfSyncComponent>();
            component1.Property = 666;
            clientEntity.Save(component1);

            var component2 = clientEntity.Get<SelfSyncComponent>();
            component2.Property = 123;
            clientEntity.Save(component2);

            var component3 = clientEntity.Get<SelfSyncComponent>();

            Assert.AreEqual(123, component2.Property);
            Assert.AreEqual(123, component3.Property);
        }

        [StructLayout(LayoutKind.Sequential)]
        [SyncedComponent(OnlyMine = true)]
        internal struct SelfSyncComponentMagicTracker : IComponent
        {
            private int _v;
            public int Property { get => _v; set => _v = value; }
        }
    }
}