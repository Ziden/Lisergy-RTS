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
            GameSpec spec = new GameSpec(1);
           

            // BUILDINGS
            // CASTLE
            spec.Buildings[1] = new BuildingSpec(
               1,
                1,
               4
            );
            // FARM
            spec.Buildings[2] = new BuildingSpec(
               2,
                2,
                4
            );

            TestTiles.Generate(ref spec);
            TestUnitData.Generate(ref spec);
            TestItems.Generate(ref spec);
            TestLoots.Generate(ref spec);
            TestDungeons.Generate(ref spec);
            spec.InitialBuilding = spec.Buildings[1].Id;
            spec.InitialUnit = TestUnitData.MAGE;
            return spec;
        }
    }
}
