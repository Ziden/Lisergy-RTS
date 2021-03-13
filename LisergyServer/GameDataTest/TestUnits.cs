using Game.Entity;
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
                Stats = new UnitStats(new Dictionary<Stat, ushort>()
                {
                    { Stat.SPEED, 10 },
                    { Stat.ACCURACY, 10 },
                    { Stat.DEF, 1 },
                    { Stat.MDEF, 5 },
                    { Stat.ATK, 1 },
                    { Stat.MATK, 5 },
                    { Stat.HP, 10 },
                    { Stat.MHP, 20 },
                    { Stat.MP, 5 },
                    { Stat.MMP, 5 },
                })
            };
        }
    }
}
