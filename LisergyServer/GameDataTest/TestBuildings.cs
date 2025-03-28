using Game.Systems.Resources;
using GameData;
using GameData.Specs;
using System.Linq;

namespace GameDataTest
{
    public class TestBuildings
    {
        private static NodeTree<BuildingSpecId> TechNode(BuildingSpecId building, params NodeTree<BuildingSpecId>[] children)
        {
            var parent = new NodeTree<BuildingSpecId>(building);
            foreach (var child in children)
            {
                parent.AddChild(child);
            }
            return parent;
        }

        private static string Addr(string name) => $"res://Content/Buildings/{name}.tscn";

        public static readonly BuildingSpecId CAMP = 1;
        public static readonly BuildingSpecId CASTLE = 2;
        public static readonly BuildingSpecId FARM = 3;
        public static readonly BuildingSpecId WELL = 4;
        public static readonly BuildingSpecId TAVERN = 5;

        public static void GenerateConstruction(ref GameSpec spec)
        {
            spec.ConstructionTechTree.Root =
                TechNode(CAMP, 
                    TechNode(WELL,
                        TechNode(TAVERN),
                        TechNode(FARM,
                            TechNode(CASTLE)
                )));

            spec.BuildingConstructions[CAMP] = new BuildingConstructionSpec(CAMP)
            {
                Icon = "res://Content/Art/Sprites/Icons/MagicItems/MagicItems_png/bg/bag_09_b.PNG",
                SpecId = CAMP,
                TimeToBuildSeconds = 10,
                BuildingCost = new ResourceStackData[] { new ResourceStackData(TestResources.LOGS.SpecId, 20) }.ToList()
            };

            spec.BuildingConstructions[FARM] = new BuildingConstructionSpec(FARM)
            {
                Icon = "res://Content/Art/Sprites/Blocks/item/wheat.png",
                SpecId = FARM,
                TimeToBuildSeconds = 10,
                BuildingCost = new ResourceStackData[] {
                    new ResourceStackData(TestResources.LOGS.SpecId, 10),
                    new ResourceStackData(TestResources.WATER.SpecId, 3),
                    new ResourceStackData(TestResources.STONE.SpecId, 5) }
                .ToList()
            };

            spec.BuildingConstructions[CASTLE] = new BuildingConstructionSpec(FARM)
            {
                Icon = "res://Content/Art/Sprites/Castle.png",
                SpecId = CASTLE,
                TimeToBuildSeconds = 10,
                BuildingCost = new ResourceStackData[] {
                    new ResourceStackData(TestResources.LOGS.SpecId, 10),
                    new ResourceStackData(TestResources.WATER.SpecId, 3),
                    new ResourceStackData(TestResources.STONE.SpecId, 5) }
               .ToList()
            };

            spec.BuildingConstructions[WELL] = new BuildingConstructionSpec(FARM)
            {
                Icon = "res://Content/Art/Sprites/Castle.png",
                SpecId = WELL,
                TimeToBuildSeconds = 10,
                BuildingCost = new ResourceStackData[] {
                    new ResourceStackData(TestResources.LOGS.SpecId, 10),
                    new ResourceStackData(TestResources.WATER.SpecId, 3),
                    new ResourceStackData(TestResources.STONE.SpecId, 5) }
            .ToList()
            };

            spec.BuildingConstructions[TAVERN] = new BuildingConstructionSpec(FARM)
            {
                Icon = "res://Content/Art/Sprites/Castle.png",
                SpecId = TAVERN,
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
                Art = new ArtSpec() { Address = Addr("Castle") },
                Description = "Produces food over time. Best near water.",
            };
            spec.Buildings[WELL] = new BuildingSpec(WELL)
            {
                Name = "Storage",
                LOS = 4,
                Art = new ArtSpec() { Address = Addr("Castle") },
                Description = "Produces food over time. Best near water.",
            };
            spec.Buildings[TAVERN] = new BuildingSpec(TAVERN)
            {
                Name = "TAVERN",
                LOS = 4,
                Art = new ArtSpec() { Address = Addr("Castle") },
                Description = "Produces food over time. Best near water.",
            };
            GenerateConstruction(ref spec);
        }
    }
}
