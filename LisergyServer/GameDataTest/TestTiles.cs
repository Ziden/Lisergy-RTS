using GameData;
using GameData.Specs;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameDataTest
{
    public class TestTiles
    {
        public static readonly TileSpec GRASS = new TileSpec()
        {
            ID=0,
            Art = new ArtSpec()
            {
                SpriteName = "wool_colored_green"
            }
        };

        public static readonly TileSpec MOUNTAIN = new TileSpec()
        {
            ID = 1,
            Art = new ArtSpec()
            {
                SpriteName = "wool_colored_red"
            }
        };

        public static readonly TileSpec WATER = new TileSpec()
        {
            ID = 2,
            Art = new ArtSpec()
            {
                SpriteName = "wool_colored_blue"
            }
        };

        public static readonly TileSpec FOREST = new TileSpec()
        {
            ID = 3,
            Art = new ArtSpec()
            {
                SpriteName = "log_oak"
            }
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
