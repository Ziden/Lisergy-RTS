using System;
using System.Collections.Generic;
using Game.Engine.DataTypes;
using NUnit.Framework;

namespace GameUnitTests;

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