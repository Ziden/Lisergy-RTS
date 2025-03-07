﻿using GameData;
using GameData.Specs;

namespace GameDataTest
{
    public class TestItems
    {
        public static ItemSpec GOLD = new ItemSpec()
        {
            Id = 13,
            Type = ItemType.RESOURCE,
            Name = "Gold",
            Art = new ArtSpec() { Address = "items_13", Type = ArtType.SPRITE_SHEET }
        };

        public static ItemSpec ORE = new ItemSpec()
        {
            Id = 15,
            Type = ItemType.RESOURCE,
            Name = "Ore",
            Art = new ArtSpec() { Address = "items_15", Type = ArtType.SPRITE_SHEET }
        };

        public static void Generate(ref GameSpec spec)
        {
            spec.Items[13] = GOLD;
            spec.Items[15] = ORE;
        }
    }
}
