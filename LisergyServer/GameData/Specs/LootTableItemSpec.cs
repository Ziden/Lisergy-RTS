
using System;
using System.Collections.Generic;

namespace GameData.Specs
{
    [Serializable]
    public class LootTableItemSpec
    {
        public ushort ItemSpecID;
        public double Chance;
        public byte Group;
    }

    [Serializable]
    public class LootSpec
    {
        public ushort SpecID;
        public List<LootTableItemSpec> LootTables;

        public LootSpec(ushort id)
        {
            this.SpecID = id;
            this.LootTables = new List<LootTableItemSpec>();
        }
    }

}
