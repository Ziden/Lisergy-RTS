using Game;
using Game.ECS;
using Game.Entity.Components;
using Game.Entity.Entities;
using Game.Events.GameEvents;
using Game.World.Components;
using Game.World.Data;
using NUnit.Framework;


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