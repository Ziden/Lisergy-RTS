using GameData;
using System;

namespace GameDataTest
{
    public class TestSpecs
    {
        public static GameSpec Generate()
        {
            var spec = new GameSpec(1);
            TestBuildings.Generate(ref spec);
            TestTiles.Generate(ref spec);
            TestUnitData.Generate(ref spec);
            TestItems.Generate(ref spec);
            TestLoots.Generate(ref spec);
            TestDungeons.Generate(ref spec);
            TestResources.Generate(ref spec);
            TestHarvestingSpots.Generate(ref spec);
            TestEntitySpecs.Generate(ref spec);
            spec.InitialBuildingSpecId = null;//spec.Buildings[1].Id;
            spec.InitialUnitSpecId = TestUnitData.MAGE;
            return spec;
        }
    }
}
