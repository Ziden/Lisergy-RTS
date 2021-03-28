using Game.Entity;
using GameData;
using GameData.buffs;
using GameData.Specs;
using System.Collections.Generic;

namespace GameDataTest
{
    public class TestUnits
    {
        private static void AddUnit(GameSpec spec, UnitSpec unitSpec)
        {
            var id = (ushort)spec.Units.Count;
            unitSpec.UnitSpecID = id;
            spec.Units[id] = unitSpec;
        }

        public static void Generate(GameSpec spec)
        {
            AddUnit(spec, new UnitSpec()
            {
                Art = new ArtSpec()
                {
                    Type = ArtType.PREFAB,
                    Name = "Mage"
                },
                Name = "Mage",
                LOS = 3,
                Stats = new UnitStats(new Dictionary<Stat, ushort>()
                {
                    { Stat.SPEED, 5 },
                    { Stat.ACCURACY, 10 },
                    { Stat.DEF, 1 },
                    { Stat.MDEF, 5 },
                    { Stat.ATK, 1 },
                    { Stat.MATK, 5 },
                    { Stat.HP, 10 },
                    { Stat.MHP, 10 },
                    { Stat.MP, 20 },
                    { Stat.MMP, 20 },
                })
            });
            AddUnit(spec, new UnitSpec()
            {
                Art = new ArtSpec()
                {
                    Type = ArtType.PREFAB,
                    Name = "Knight"
                },
                Name = "Knight",
                LOS = 1,
                Stats = new UnitStats(new Dictionary<Stat, ushort>()
                {
                    { Stat.SPEED, 8 },
                    { Stat.ACCURACY, 5 },
                    { Stat.DEF, 4 },
                    { Stat.MDEF, 4 },
                    { Stat.ATK, 3 },
                    { Stat.MATK, 2 },
                    { Stat.HP, 20 },
                    { Stat.MHP, 20 },
                    { Stat.MP, 6 },
                    { Stat.MMP, 6 },
                })
            });
        }
    }
}
