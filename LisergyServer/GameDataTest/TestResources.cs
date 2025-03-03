using GameData;
using GameData.Specs;

namespace GameDataTest
{
    public class TestResources
    {
        public static readonly ResourceSpec LOGS = new ResourceSpec(0)
        {
            Name = "Logs",
            WeightPerUnit = 5,
            Art = new ArtSpec() { Address = "Assets/Addressables/Sprites/Icons/ResourcesAndCraftIcons/ResourcesAndCraftIcons_png/transparent/wood/wd_t_03.png", Type = ArtType.SPECIFIC_SPRITE }
        };

        public static readonly ResourceSpec WATER = new ResourceSpec(1)
        {
            Name = "Water",
            WeightPerUnit = 1,
            Art = new ArtSpec() { Address = "Assets/Addressables/Sprites/Icons/FoodIconPack/bucket_01.png", Type = ArtType.SPECIFIC_SPRITE }
        };

        public static readonly ResourceSpec FOOD = new ResourceSpec(2)
        {
            Name = "Food",
            WeightPerUnit = 2,
            Art = new ArtSpec() { Address = "Assets/Addressables/Sprites/Icons/FoodIconPack/baking_04.png", Type = ArtType.SPECIFIC_SPRITE }
        };

        public static readonly ResourceSpec STONE = new ResourceSpec(3)
        {
            Name = "Stone",
            WeightPerUnit = 10,
            Art = new ArtSpec() { Address = "Assets/Addressables/Sprites/Icons/ResourcesAndCraftIcons/ResourcesAndCraftIcons_png/transparent/stone/st_t_01.PNG", Type = ArtType.SPECIFIC_SPRITE }
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
