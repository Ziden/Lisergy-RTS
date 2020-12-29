using GameData;
using GameData.Specs;
using System;

namespace GameDataTest
{
    [Flags]
    public enum ByteFlags : byte
    {
        F1 = 1 << 0,
        F2 = 1 << 1,
        F3 = 1 << 2,
        F4 = 1 << 3,
        F5 = 1 << 4,
        F6 = 1 << 5,
        F7 = 1 << 6,
        F8 = 1 << 7
    }

    public class TestSpecs
    {
        public static GameSpec Generate()
        {
            GameSpec spec = new GameSpec();

            // BUILDINGS
            // CASTLE
            spec.Buildings[1] = new BuildingSpec() {
                Id = 1,
                ModelID = 1,
            };
            // FARM
            spec.Buildings[2] = new BuildingSpec() {
                Id = 2,
                ModelID = 2,
            };
            spec.InitialBuilding = spec.Buildings[1].Id;

            TestTiles.Generate(spec);
            TestUnits.Generate(spec);
            return spec;
        }
    }
}
