using System.Collections.Generic;
using GameData;
using GameData.Specs;

namespace GameDataTest
{
	public class TestDungeons
	{
		public static DungeonSpec EASY = new DungeonSpec
		{
			Art = new ArtSpec {Type = ArtType.PREFAB, Address = Addr("dungeon")},
			LootSpecID = TestLoots.GOLD_GUARANTEED.SpecID,
			SpecId = 0,
			BattleSpecs = new List<BattleSpec>
			{
				new BattleSpec
				{
					UnitSpecIDS = new UnitSpecId[1] {TestUnitData.THIEF}
				}
			}
		};

		private static string Addr(string name)
		{
			return $"res://Content/Buildings/{name}.tscn";
		}

		public static void Generate(ref GameSpec spec)
		{
			spec.Dungeons[0] = EASY;
		}
	}
}