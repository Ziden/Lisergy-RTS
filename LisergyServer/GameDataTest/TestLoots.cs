using GameData;
using GameData.Specs;

namespace GameDataTest
{
    public class TestLoots
    {
        public static LootSpec GOLD_GUARANTEED;

        public static void Generate(ref GameSpec spec)
        {
            GOLD_GUARANTEED = new LootSpec(0);
            GOLD_GUARANTEED.LootTables.Add(new LootTableItemSpec()
            {
                Chance = 1.0,
                ItemSpecID = TestItems.GOLD.Id
            });
            spec.Loots[0] = GOLD_GUARANTEED;
        }
    }
}
