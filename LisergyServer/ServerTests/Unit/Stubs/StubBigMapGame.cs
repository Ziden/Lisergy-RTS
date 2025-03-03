using Game.Engine.DataTypes;
using Game.World;
using GameDataTest.TestWorldGenerator;

namespace ServerTests
{
    public class TestLargeGame : TestGame
    {
        protected override GameWorld CreateTestWorld()
        {
            GameId.INCREMENTAL_MODE = 1;
            WorldUtils.SetRandomSeed(666);
            var world = new TestWorld(this);
            SetupWorld(world);
            Network.DeltaCompression.ClearDeltas();
            return world;
        }
    }

}
