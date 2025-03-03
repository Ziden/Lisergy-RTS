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

        public static void GenerateConstruction(ref GameSpec spec)
        {
            spec.Construction.Root = new BuildingTechTreeNode()
            {
                Building = CAMP
            };

            spec.BuildingConstructions[CAMP] = new BuildingConstructionSpec(CAMP)
            {
                Icon = Addr("Camp"),
                SpecId = CAMP,
                TimeToBuildSeconds = 10,
                BuildingCost = new ResourceStackData[] { new ResourceStackData(TestResources.LOGS.SpecId, 20) }.ToList()
            };

            spec.BuildingConstructions[FARM] = new BuildingConstructionSpec(FARM)
            {
                Icon = AddrIcons("Farm"),
                SpecId = FARM,
                TimeToBuildSeconds = 10,
                BuildingCost = new ResourceStackData[] {
                    new ResourceStackData(TestResources.LOGS.SpecId, 10),
                    new ResourceStackData(TestResources.WATER.SpecId, 3),
                    new ResourceStackData(TestResources.STONE.SpecId, 5) }
                .ToList()
            };
        }

        public static void Generate(ref GameSpec spec)
        {
            spec.Buildings[CAMP] = new BuildingSpec(CAMP)
            {
                Name = "Camp",
                LOS = 4,
                Description = "Can heal units and store resources.",
                Art = Addr("Camp"),
            };
            spec.Buildings[CASTLE] = new BuildingSpec(CASTLE)
            {
                Name = "Castle",
                LOS = 4,
                Art = new ArtSpec() { Address = Addr("Castle") },
                Description = "Can heal units and store resources. Contains defensive bonuses.",
            };
            spec.Buildings[FARM] = new BuildingSpec(FARM)
            {
                Name = "Farm",
                LOS = 4,
                Art = new ArtSpec() { Address = Addr("Farm") },
                Description = "Produces food over time. Best near water.",
            };
        }
    }
}
