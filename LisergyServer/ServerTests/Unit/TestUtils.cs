using Game;
using Game.DataTypes;
using Game.Network.ClientPackets;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace Tests
{
    public class TestUtils
    { 
        [Test]
        public void TestDefaultDictionary()
        {
            var d = new DefaultValueDictionary<Type, List<string>>();
            d[typeof(int)].Add("int type");

            Assert.That(d[typeof(int)].Count, Is.EqualTo(1));
        }
    }
}