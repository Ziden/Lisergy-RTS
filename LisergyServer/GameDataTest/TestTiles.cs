using GameData;
using GameData.Specs;
using System.Collections.Generic;

namespace GameDataTest
{
    public class TestTiles
    {
        public static readonly TileSpec GRASS = new TileSpec()
        {
            ID=0,
            MovementFactor = 1,
            Arts = new List<ArtSpec>(new ArtSpec[] {
                  new ArtSpec() { Name = "Plains", Type=ArtType.PREFAB }
            })
        };

        public static readonly TileSpec MOUNTAIN = new TileSpec()
        {
            ID = 1,
            MovementFactor = 0,
            Arts = new List<ArtSpec>(new ArtSpec[] {
                 new ArtSpec() { Name = "Mountain", Type=ArtType.PREFAB }
            })
        };

        public static readonly TileSpec WATER = new TileSpec()
        {
            ID = 2,
            MovementFactor = 0.5f,
            Arts = new List<ArtSpec>(new ArtSpec[] {
                   new ArtSpec() { Name = "Water", Type=ArtType.PREFAB }
            })
        };

        public static readonly TileSpec FOREST = new TileSpec()
        {
            MovementFactor = 0.8f,
            ID = 3,
            Arts = new List<ArtSpec>(new ArtSpec[] {
                new ArtSpec() { Name = "Forest", Type=ArtType.PREFAB }
            })
        };

        public static void Generate(GameSpec spec)
        {
            spec.Tiles[0] = GRASS;
            spec.Tiles[1] = MOUNTAIN;
            spec.Tiles[2] = WATER;
            spec.Tiles[3] = FOREST;
        }
    }
}
