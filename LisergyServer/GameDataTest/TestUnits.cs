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
                    { Stat.SPEED, 5 },
                    { Stat.ACCURACY, 5 },
                    { Stat.DEF, 5 },
                    { Stat.MDEF, 5 },
                    { Stat.ATK, 5 },
                    { Stat.MATK, 5 },
                    { Stat.MHP, 20 },
                    { Stat.MMP, 19 },
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
                    (Stat.SPEED, 2),
                    (Stat.DEF, -2),
                    (Stat.MDEF, -1),
                    (Stat.ATK, 1),
                    (Stat.MATK, -2)
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
                    (Stat.ATK, 1),
                    (Stat.MDEF, -1),
                    (Stat.SPEED, -3),
                    (Stat.DEF, 2),
                    (Stat.ACCURACY, 1),
                    (Stat.MATK, -1),
                    (Stat.MHP, 10)
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
                    (Stat.ATK, -2),
                    (Stat.MATK, 2),
                    (Stat.MDEF, 1),
                    (Stat.SPEED, -1),
                    (Stat.DEF, -2),
                    (Stat.ACCURACY, 2),
                    (Stat.MHP, -5),
                    (Stat.MMP, 10)
                )
            });
        }
    }
}
