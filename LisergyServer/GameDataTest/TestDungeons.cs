using GameData;
using GameData.Specs;
using System.Collections.Generic;

namespace GameDataTest
{
    public class TestDungeons
    {
        public static DungeonSpec EASY = new DungeonSpec()
        {
            Art = new ArtSpec() { Type = ArtType.PREFAB, Name = "buildings/dungeon" },
            LootSpecID = TestLoots.GOLD_GUARANTEED.SpecID,
            DungeonSpecID = 0,
            BattleSpecs = new List<BattleSpec>()
        };

        public static void Generate(GameSpec spec)
        {
            EASY.BattleSpecs.Add(new BattleSpec()
            {
                UnitSpecIDS = new ushort[1] { TestUnits.THIEF }
            });
            spec.Dungeons[0] = EASY;
        }
    }
}
