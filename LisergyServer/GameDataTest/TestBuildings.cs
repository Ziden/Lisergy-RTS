using Game.Systems.Resources;
using GameData;
using GameData.Specs;
using System.Linq;

namespace GameDataTest
{
    public class TestBuildings
    {
        private static string Addr(string name) => $"Assets/Addressables/Prefabs/Buildings/{name}.prefab";
        private static string AddrIcons(string name) => $"Assets/Addressables/Prefabs/Buildings/{name}.prefab";

        public static readonly BuildingSpecId CAMP = 1;
        public static readonly BuildingSpecId CASTLE = 2;
        public static readonly BuildingSpecId FARM = 3;

        public static void Generate(ref GameSpec spec)
        {
            spec.Buildings[CAMP] = new BuildingSpec(CAMP)
            {
                Name = "Camp",
                LOS = 4,
                Icon = Addr("Camp"),
                Art = Addr("Camp"),
                BuildingCost = new ResourceStackData[] { new ResourceStackData(TestResources.LOGS.SpecId, 20) }.ToList()
            };
            spec.Buildings[CASTLE] = new BuildingSpec(CASTLE)
            {
                Name = "Castle",
                LOS = 4,
                Art = new ArtSpec() { Address = Addr("Castle") },
                BuildingCost = new ResourceStackData[] { new ResourceStackData(TestResources.LOGS.SpecId, 11110)}.ToList()
            };
            spec.Buildings[FARM] = new BuildingSpec(FARM)
            {
                Name = "Farm",
                LOS = 4,
                Art = new ArtSpec() { Address = Addr("Farm") },
                BuildingCost = new ResourceStackData[] { new ResourceStackData(TestResources.LOGS.SpecId, 11110) }.ToList()
            };
        }
    }
}
