using GameData;
using GameData.Specs;
using System.Collections.Generic;

namespace GameDataTest
{
    public class TestDungeons
    {
        private static string Addr(string name) => $"Assets/Addressables/Prefabs/Buildings/{name}.prefab";

        public static DungeonSpec EASY = new DungeonSpec()
        {
            Art = new ArtSpec() { Type = ArtType.PREFAB, Address = Addr("dungeon") },
            LootSpecID = TestLoots.GOLD_GUARANTEED.SpecID,
            DungeonSpecID = 0,
            BattleSpecs = new List<BattleSpec>()
        };

        public static void Generate(ref GameSpec spec)
        {
            EASY.BattleSpecs.Add(new BattleSpec()
            {
                UnitSpecIDS = new ushort[1] { TestUnitData.THIEF }
            });
            spec.Dungeons[0] = EASY;
        }
    }
}
