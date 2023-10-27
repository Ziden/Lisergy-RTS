using GameData;
using GameData.Specs;
using System.Collections.Generic;

namespace GameDataTest
{
    public class TestResources
    {
        private static string Addr(string name) => $"Assets/Addressables/Prefabs/Resources/{name}.prefab";
        
        public static readonly ResourceSpec LOGS = new ResourceSpec(0)
        {
           Name = "Logs",
           WeightPerUnit = 5,
           Art = new ArtSpec() { Address = Addr("Logs"), Type=ArtType.PREFAB }
        };

        public static readonly ResourceSpec WATER = new ResourceSpec(1)
        {
            Name = "Water",
            WeightPerUnit = 1,
            Art = new ArtSpec() { Address = Addr("Water"), Type=ArtType.PREFAB }
        };
        
        public static readonly ResourceSpec FOOD = new ResourceSpec(2)
        {
            Name = "Food",
            WeightPerUnit = 2,
            Art = new ArtSpec() { Address = Addr("Food"), Type=ArtType.PREFAB }
        };

        public static readonly ResourceSpec STONE = new ResourceSpec(3)
        {
            Name = "Stone",
            WeightPerUnit = 10,
            Art = new ArtSpec() { Address = Addr("Stone"), Type=ArtType.PREFAB }
        };

        public static void Generate(ref GameSpec spec)
        {
            spec.Resources[0] = LOGS;
            spec.Resources[1] = WATER;
            spec.Resources[2] = FOOD;
            spec.Resources[3] = STONE;
        }
    }
}
