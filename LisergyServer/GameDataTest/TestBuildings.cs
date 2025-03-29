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
        public static readonly BuildingSpecId FORT = 2;
        public static readonly BuildingSpecId FARM = 3;
        public static readonly BuildingSpecId LUMBER_CAMP = 4;
        public static readonly BuildingSpecId TAVERN = 5;

        public static void GenerateConstruction(ref GameSpec spec)
        {
            spec.ConstructionTechTree.Root =
                TechNode(CAMP, 
                    TechNode(LUMBER_CAMP,
                        TechNode(TAVERN),
                        TechNode(FARM,
                            TechNode(FORT)
                )));

            spec.BuildingConstructions[CAMP] = new BuildingConstructionSpec(CAMP)
            {
                Icon = "res://Content/Art/Sprites/Icons/MagicItems/MagicItems_png/bg/bag_09_b.PNG",
                SpecId = CAMP,
                TimeToBuildSeconds = 10,
                Costs = new ResourceStackData[] { new ResourceStackData(TestResources.LOGS.SpecId, 20) }.ToList()
            };

            spec.BuildingConstructions[FARM] = new BuildingConstructionSpec(FARM)
            {
                Icon = "res://Content/Art/Sprites/Icons/Herbal_Icons/Herbal_Icons_png/Addons/11_b.PNG",
                SpecId = FARM,
                TimeToBuildSeconds = 10,
                Costs = new ResourceStackData[] {
                    new ResourceStackData(TestResources.LOGS.SpecId, 10),
                    new ResourceStackData(TestResources.WATER.SpecId, 3),
                    new ResourceStackData(TestResources.STONE.SpecId, 5) }
                .ToList()
            };

            spec.BuildingConstructions[FORT] = new BuildingConstructionSpec(FARM)
            {
                Icon = "res://Content/Art/Sprites/Icons/WeaponAndArmorIcons/WeaponAndArmorIcons_png/black/addons/013_b.png",
                SpecId = FORT,
                TimeToBuildSeconds = 10,
                Costs = new ResourceStackData[] {
                    new ResourceStackData(TestResources.LOGS.SpecId, 10),
                    new ResourceStackData(TestResources.WATER.SpecId, 3),
                    new ResourceStackData(TestResources.STONE.SpecId, 5) }
               .ToList()
            };

            spec.BuildingConstructions[LUMBER_CAMP] = new BuildingConstructionSpec(FARM)
            {
                Icon = "res://Content/Art/Sprites/Icons/ResourcesAndCraftIcons/ResourcesAndCraftIcons_png/black/wood/wd_b_07.png",
                SpecId = LUMBER_CAMP,
                TimeToBuildSeconds = 10,
                Costs = new ResourceStackData[] {
                    new ResourceStackData(TestResources.LOGS.SpecId, 10),
                    new ResourceStackData(TestResources.WATER.SpecId, 3),
                    new ResourceStackData(TestResources.STONE.SpecId, 5) }
            .ToList()
            };

            spec.BuildingConstructions[TAVERN] = new BuildingConstructionSpec(FARM)
            {
                Icon = "res://Content/Art/Sprites/Icons/LootIcons/LootIcons_png/black/mug_b_01.png",
                SpecId = TAVERN,
                TimeToBuildSeconds = 10,
                Costs = new ResourceStackData[] {
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
                Name = "Campsite",
                LOS = 4,
                Description = "Can heal units and store resources.",
                Art = Addr("Camp"),
            };
            spec.Buildings[FORT] = new BuildingSpec(FORT)
            {
                Name = "Fort",
                LOS = 4,
                Art = new ArtSpec() { Address = Addr("Fort") },
                Description = "Can heal units and store resources. Contains defensive bonuses.",
            };
            spec.Buildings[FARM] = new BuildingSpec(FARM)
            {
                Name = "Farm",
                LOS = 4,
                Art = new ArtSpec() { Address = Addr("Farm") },
                Description = "Produces food over time. Best near water.",
            };
            spec.Buildings[LUMBER_CAMP] = new BuildingSpec(LUMBER_CAMP)
            {
                Name = "MILL",
                LOS = 4,
                Art = new ArtSpec() { Address = Addr("LumberCamp") },
                Description = "Produces food over time. Best near water.",
            };
            spec.Buildings[TAVERN] = new BuildingSpec(TAVERN)
            {
                Name = "Tavern",
                LOS = 4,
                Art = new ArtSpec() { Address = Addr("Tavern") },
                Description = "Produces food over time. Best near water.",
            };
            GenerateConstruction(ref spec);
        }
    }
}
