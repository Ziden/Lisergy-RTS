﻿using Game;
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
        public void CheckNull()
        {
            GameId id1 = null;
            GameId id2 = Guid.Empty;

            Assert.AreEqual(id1, id2);
        }

        [Test]
        public void CheckNullComparisson()
        {
            GameId nu = null;

            Assert.IsTrue(nu == null);
        }

        [Test]
        public void CheckNotNullComparisson()
        {
            GameId nu = GameId.Generate();

            Assert.IsTrue(nu != null);
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
        public void CheckStringAssignment()
        {
            var guid = Guid.NewGuid().ToString();
            GameId id1 = guid;
            GameId id2 = guid;

            Assert.IsTrue(id1.IsEqualsTo(id2));
        }

        [Test]
        public void TestStringDeffer()
        {
            var guid = Guid.NewGuid().ToString();
            GameId id1 = guid;

            string str = id1;

            Assert.AreEqual(str, guid);
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