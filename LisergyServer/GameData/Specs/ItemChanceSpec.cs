
using System;
using System.Collections.Generic;

namespace GameData.Specs
{
    [Serializable]
    public struct ItemChanceSpec
    {
        public ushort ItemSpecID;
        public double Chance;
        public byte Group;
    }

    [Serializable]
    public struct LootSpec
    {
        public ushort SpecID;
        public Dictionary<byte, ItemChanceSpec> LootGroups;

        public LootSpec(ushort id)
        {
            this.SpecID = id;
            this.LootGroups = new Dictionary<byte, ItemChanceSpec>();
        }
    }

}
