using GameData.buffs;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameData.Specs
{
    [Serializable]
    public class UnitSpec
    {
        public string Name;
        public ushort UnitSpecID;
        public byte LOS;
        public ArtSpec Art;

        public Dictionary<Stats, int> Stats = new Dictionary<Stats, int>();
    }
}
