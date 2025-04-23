using Game.Engine.ECLS;
using Game.Systems.Resources;
using NUnit.Framework;
using ServerTests;

namespace GameUnitTests;

public class TestCargo
{
	private TestGame _game;
	private IEntity _party;
	private TestServerPlayer _player;

	[SetUp]
	public void Setup()
	{
		_game = new TestGame();
		_player = _game.GetTestPlayer();
		_party = _player.GetParty(0);
	}

	[Test]
	public void TestAddToCargo()
	{
		_party.Logic.Cargo.AddTocargo(new ResourceStackData(0, 10));

		var cargo = _party.Get<CargoComponent>();

		Assert.AreEqual(10, cargo.GetAmount(0));
		Assert.IsTrue(cargo.CurrentWeight > 0);
	}
}