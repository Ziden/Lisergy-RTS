using Game;
using Game.DataTypes;
using Game.Network;
using Game.Scheduler;
using Game.Services;
using Game.Systems.Player;
using Game.Tile;
using Game.World;
using GameData;
using GameDataTest;
using GameDataTest.TestWorldGenerator;
using System;
using System.Collections.Generic;

namespace ServerTests
{
    public class TestLargeGame : TestGame
    {
        protected override GameWorld CreateTestWorld()
        {
            GameId.DEBUG_MODE = 1;
            WorldUtils.SetRandomSeed(666);
            TestWorld = new TestWorld(this);
            SetupWorld(TestWorld);
            Entities.DeltaCompression.ClearDeltas();
            TestMap = TestWorld.Map as ServerChunkMap;
            return TestWorld;
        }
    }

}
