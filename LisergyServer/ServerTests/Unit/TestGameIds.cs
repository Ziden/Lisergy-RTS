using Game;
using Game.Entity;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerTests.Unit
{
    public class TestGameIds
    {
        [Test]
        public void CheckNotEqual()
        {
            var id1 = GameId.Generate();
            var id2 = GameId.Generate();

            Assert.AreNotEqual(id1, id2);
        }

        [Test]
        public void CheckNotNullComparisson()
        {
            GameId nu = GameId.Generate();

            Assert.IsTrue(nu != null);
        }


        [Test]
        public void TestGuidBackForth()
        {
            var guid = Guid.NewGuid();
            GameId id1 = guid;
            Guid back = id1;
      

            Assert.AreEqual(guid, back);
        }

        public class TestClass
        {
            public GameId Zero;
        }

        [Test]
        public void TestUnassigned()
        {
            var c = new TestClass();

            Assert.IsTrue(c.Zero == Guid.Empty);
        }

        [Test]
        public void TestZero()
        {
            var guid = Guid.Empty;
            GameId id1 = guid;
            

            Assert.IsTrue(id1.IsZero());
        }

        [Test]
        public void TestZeroAssignments()
        {
            GameId zero1 = Guid.Empty;
            GameId zero2 = Guid.Empty;
            zero2 = zero1;
            Assert.IsTrue(zero1 == zero2);
        }


        [Test]
        public void TestZeroInit()
        {
            GameId zero = Guid.Empty;

            Assert.IsTrue(zero.IsZero());
        }

        [Test]
        public void TestNonInitialized()
        {
            GameId zero = default;

            Assert.IsTrue(zero.IsZero());
        }

        [Test]
        public void CheckEqual()
        {
            var id1 = GameId.Generate();
            var id2 = id1;

            Assert.AreEqual(id1, id2);
        }

        [Test]
        public void CheckEqualOperator()
        {
            var guid = Guid.NewGuid();
            GameId id1 = guid;
            GameId id2 = guid;

            Assert.IsTrue(id1.IsEqualsTo(id2));
        }

        [Test]
        public void CheckNotEqualOperator()
        {
            GameId id1 = Guid.NewGuid();
            GameId id2 = Guid.NewGuid();

            Assert.IsTrue(id1 != id2);
        }
    }
}
