using GameData;
using GameData.Specs;

namespace GameDataTest
{
    public class TestTiles
    {
        private static string Addr(string name) => $"res://Content/Tiles/{name}.tscn";

        public static readonly TileSpec GRASS = new TileSpec(0)
        {
            Name = "Plains",
            MovementFactor = 1,
            Model = new ArtSpec(Addr("Plains")),
            Icon = new ArtSpec("res://Content/Art/Sprites/Icons/FoodIconPack/grass_01.png")
            {
                Type = ArtType.SPECIFIC_SPRITE
            }
        };

        public static readonly TileSpec MOUNTAIN = new TileSpec(1)
        {
            Name = "Rocks",
            MovementFactor = 0,
            ResourceSpotSpecId = TestHarvestingSpots.MOUNTAIN.SpecId,
            ChangeToTileIdWhenDepleted = GRASS.ID,
            Model = new ArtSpec(Addr("Mountain")),
            Icon = new ArtSpec("res://Content/Art/Ui/UiArt/Sprites/Component/Icon_EquipmentIcons_(Original)/equip_stone.png")
            {
                Type = ArtType.SPECIFIC_SPRITE
            }
        };

        public static readonly TileSpec WATER = new TileSpec(2)
        {
            Name = "Water",
            MovementFactor = 0.5f,
            ResourceSpotSpecId = TestHarvestingSpots.RIVER.SpecId,
            ChangeToTileIdWhenDepleted = GRASS.ID,
            Model = new ArtSpec() { Address = Addr("Water"), Type = ArtType.PREFAB },
            Icon = new ArtSpec()
            {
                Address = "res://Content/Art/Sprites/Icons/FoodIconPack/water_01.png",
                Type = ArtType.SPECIFIC_SPRITE
            }
        };

        public static readonly TileSpec FOREST = new TileSpec(3)
        {
            Name = "Forest",
            MovementFactor = 0.8f,
            ResourceSpotSpecId = TestHarvestingSpots.LOGS.SpecId,
            ChangeToTileIdWhenDepleted = GRASS.ID,
            Model = new ArtSpec() { Address = Addr("Forest"), Type = ArtType.PREFAB },
            Icon = new ArtSpec()
            {
                Address = "res://Content/Art/Sprites/Pack/textures/item/acacia_sapling.png",
                Type = ArtType.SPECIFIC_SPRITE
            }
        };

        public static void Generate(ref GameSpec spec)
        {
            spec.Tiles[0] = GRASS;
            spec.Tiles[1] = MOUNTAIN;
            spec.Tiles[2] = WATER;
            spec.Tiles[3] = FOREST;
        }
    }
}
