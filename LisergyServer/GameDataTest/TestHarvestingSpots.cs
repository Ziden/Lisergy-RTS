using System;
using GameData;

namespace GameDataTest
{
    public class TestHarvestingSpots
    {
        private static string Addr(string name) => $"Assets/Addressables/Prefabs/Resources/{name}.prefab";
        
        public static readonly ResourceHarvestPointSpec LOGS = new ResourceHarvestPointSpec(0)
        {
            HarvestTimePerUnit = TimeSpan.FromSeconds(1),
            RespawnTime = TimeSpan.FromMinutes(1),
            ResourceId = TestResources.LOGS.SpecId,
            ResourceAmount = 10
        };

        public static readonly ResourceHarvestPointSpec RIVER = new ResourceHarvestPointSpec(1)
        {
            HarvestTimePerUnit = TimeSpan.FromSeconds(3),
            RespawnTime = TimeSpan.FromDays(7),
            ResourceId = TestResources.WATER.SpecId,
            ResourceAmount = 500
        };
        public static readonly ResourceHarvestPointSpec BERRIES = new ResourceHarvestPointSpec(2)
        {
            HarvestTimePerUnit = TimeSpan.FromSeconds(1),
            RespawnTime = TimeSpan.FromHours(1),
            ResourceId = TestResources.FOOD.SpecId,
            ResourceAmount = 5
        };
        
        public static readonly ResourceHarvestPointSpec MOUNTAIN = new ResourceHarvestPointSpec(3)
        {
            HarvestTimePerUnit = TimeSpan.FromSeconds(5),
            RespawnTime = TimeSpan.FromDays(7),
            ResourceId = TestResources.STONE.SpecId,
            ResourceAmount = 500
        };

        public static void Generate(ref GameSpec spec)
        {
            spec.HarvestPoints[0] = LOGS;
            spec.HarvestPoints[1] = RIVER;
            spec.HarvestPoints[2] = BERRIES;
            spec.HarvestPoints[3] = MOUNTAIN;
        }
    }
}
