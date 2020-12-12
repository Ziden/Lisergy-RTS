using GameData;
using System;

namespace GameDataTest
{
    public class TestSpecs
    {
        public static GameSpec Generate()
        {
            GameSpec spec = new GameSpec();

            spec.Buildings[0] = new BuildingSpec() { ModelID = 0, Name = "Castle" };
            spec.Buildings[1] = new BuildingSpec() { ModelID = 1, Name = "Farm" };

            spec.InitialBuilding = spec.Buildings[0];

            return spec;
        }
    }
}
