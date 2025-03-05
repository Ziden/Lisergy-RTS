using ClientSDK.Data;
using Game;
using Game.Engine;
using GameDataTest;
using GameDataTest.TestWorldGenerator;
using NUnit.Framework;
using ServerTests.Integration.Stubs;

namespace GameUnitTests
{
    public class TestNetworkSync
    {
        TestGameClient _testClient;

        LisergyGame _clientLogic;
        LisergyGame _serverLogic;

        [SetUp]
        public void Setup()
        {
            _testClient = new TestGameClient(null);

            var specs = TestSpecs.Generate();

            _serverLogic = new LisergyGame(specs, new GameLog("[Server]"));
            _serverLogic.SetupWorld(new TestWorld(_serverLogic));
            _serverLogic.Network.DeltaCompression.ClearDeltas();

            _clientLogic = new LisergyGame(specs, new GameLog("[Client]"), isClientGame: true);
            _clientLogic.SetupWorld(new ClientWorld(_clientLogic, 100, 100));

            _testClient.InitializeGame(_clientLogic);
        }

        [Test]
        public void TestNetwork()
        {

        }

    }
}
