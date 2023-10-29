using GameData;
using GameData.Specs;

namespace GameDataTest
{
    public class TestTiles
    {
        private static string Addr(string name) => $"Assets/Addressables/Prefabs/Tiles/{name}.prefab";
        
        public static readonly TileSpec GRASS = new TileSpec(0)
        {
            MovementFactor = 1,
            Art = new ArtSpec() { Address = Addr("Plains"), Type=ArtType.PREFAB }
        };

        public static readonly TileSpec MOUNTAIN = new TileSpec(1)
        {
            MovementFactor = 0,
            ResourceSpotSpecId = TestHarvestingSpots.MOUNTAIN.SpecId,
            ChangeToTileIdWhenDepleted = GRASS.ID,
            Art = new ArtSpec() { Address = Addr("Mountain"), Type=ArtType.PREFAB }
        };

        public static readonly TileSpec WATER = new TileSpec(2)
        {
            MovementFactor = 0.5f,
            ResourceSpotSpecId = TestHarvestingSpots.RIVER.SpecId,
            ChangeToTileIdWhenDepleted = GRASS.ID,
            Art = new ArtSpec() { Address = Addr("Water"), Type=ArtType.PREFAB }
        };

        public static readonly TileSpec FOREST = new TileSpec(3)
        {
            MovementFactor = 0.8f,
            ResourceSpotSpecId = TestHarvestingSpots.LOGS.SpecId,
            ChangeToTileIdWhenDepleted = GRASS.ID,
            Art =new ArtSpec() { Address = Addr("Forest"), Type=ArtType.PREFAB }
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
