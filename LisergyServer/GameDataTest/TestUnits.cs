using Game.Entity;
using Game.Systems.Battler;
using GameData;
using GameData.buffs;
using GameData.Specs;
using System.Collections.Generic;

namespace GameDataTest
{
    public static class TestUnitData
    {
        public static string Addr(string name) => $"Assets/Addressables/Prefabs/Units/{name}.prefab";

        public static string AddrFace(string name) => $"Assets/Addressables/Sprites/Badges/{name}.png";

        private static void AddUnit(ref GameSpec spec, UnitSpec unitSpec)
        {
            var id = (ushort)spec.Units.Count;
            unitSpec.UnitSpecID = id;
            spec.Units[id] = unitSpec;
        }

        public static readonly ushort THIEF = 0;
        public static readonly ushort KNIGHT = 1;
        public static readonly ushort MAGE = 2;

        private static UnitStats BaseStats = new UnitStats().SetStats(new Dictionary<Stat, byte>()
            {
                    { Stat.SPEED, 50 },
                    { Stat.ACCURACY, 50 },
                    { Stat.DEF, 50 },
                    { Stat.MDEF, 50 },
                    { Stat.ATK, 50 },
                    { Stat.MATK, 50 },
                    { Stat.MHP, 100 },
                    { Stat.MMP, 30 },
        });

        public static UnitStats SetStats(this UnitStats u, Dictionary<Stat, byte> stats)
        {
            foreach (var kp in stats)
                u[kp.Key] = kp.Value;
            return u;
        }


        private static UnitStats AddToBase(params (Stat, short)[] stats)
        {
            var st = new UnitStats();
            st.SetStats(BaseStats);
            foreach (var item in stats)
            {
                short value = st.GetStat(item.Item1);
                value += item.Item2;
                st[item.Item1] = (byte)value;
            }

            return st;
        }

        public static void Generate(ref GameSpec spec)
        {
            AddUnit(ref spec, new UnitSpec()
            {
                Art = new ArtSpec() { Type = ArtType.PREFAB,Address = Addr("Rogue") },
                Name = "Rogue",
                IconArt = new ArtSpec()
                {
                    Type = ArtType.SPECIFIC_SPRITE,
                    Address = AddrFace("Badge_rogue"),
                },
                LOS = 2,
                Stats = AddToBase(
                    (Stat.ACCURACY, 20),
                    (Stat.SPEED, 30),
                    (Stat.DEF, -30),
                    (Stat.MDEF, -20),
                    (Stat.ATK, 30),
                    (Stat.MATK, -30)
                )
            });
            AddUnit(ref spec, new UnitSpec()
            {
                Art = new ArtSpec() { Type = ArtType.PREFAB, Address = Addr("Knight") },
                Name = "Knight",
                LOS = 1,
                IconArt = new ArtSpec()
                {
                    Type = ArtType.SPECIFIC_SPRITE,
                    Address = AddrFace("Badge_warrior"),
                },
                Stats = AddToBase(
                    (Stat.ATK, 10),
                    (Stat.MDEF, -30),
                    (Stat.SPEED, -40),
                    (Stat.DEF, 30),
                    (Stat.ACCURACY, -10),
                    (Stat.MATK, -10),
                    (Stat.MHP, 30)
                )
            });
            AddUnit(ref spec, new UnitSpec()
            {
                Art = new ArtSpec() { Type = ArtType.PREFAB, Address = Addr("Mage") },
                Name = "Mage",
                IconArt = new ArtSpec()
                {
                    Type = ArtType.SPECIFIC_SPRITE,
                    Address = AddrFace("Badge_mage"),
                },
                LOS = 3,
                Stats = AddToBase(
                    (Stat.ATK, -30),
                    (Stat.MATK, 30),
                    (Stat.MDEF, 10),
                    (Stat.SPEED, -10),
                    (Stat.DEF, -30),
                    (Stat.ACCURACY, 20),
                    (Stat.MHP, -20),
                    (Stat.MMP, 50)
                )
            });
        }
    }
}
