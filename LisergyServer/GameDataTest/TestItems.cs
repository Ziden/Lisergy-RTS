using GameData;
using GameData.Specs;
using System.Collections.Generic;

namespace GameDataTest
{
    public class TestItems
    {
        public static ItemSpec GOLD = new ItemSpec()
        {
            Id = 0, 
            Type = ItemType.RESOURCE,
            Name = "Gold",
            Art = new ArtSpec() { Name = "item_13", Type = ArtType.SPRITE }
        };

        public static void Generate(GameSpec spec)
        {
            spec.Items[0] = GOLD;
        }
    }
}
