using GameData;
using GameData.buffs;
using GameData.Specs;
using System.Collections.Generic;

namespace GameDataTest
{
    public class TestUnits
    {
        public static void Generate(GameSpec spec)
        {
            spec.Units[0] = new UnitSpec()
            {
                Art = new ArtSpec()
                {
                    Type = ArtType.PREFAB,
                    Name = "Mage"
                },
                UnitSpecID = 0,
                Name = "Mage",
                LOS = 3,
                Stats = new Dictionary<Stats, int>()
                {
                    { Stats.AGI, 5 },
                    { Stats.DEX, 5 },
                    { Stats.INT, 5 },
                    { Stats.STR, 5 },
                    { Stats.VIT, 5 }
                }
            };
        }
    }
}
