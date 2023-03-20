using GameData.Specs;
using System;

namespace Game.Inventories
{
    [Serializable]
    public class Item
    {
        public ushort SpecID;
        public uint Amount;

        public Item(ushort specId, uint amount)
        {
            SpecID = specId;
            Amount = amount;
        }

        public ItemSpec Spec => StrategyGame.Specs.Items[SpecID];
    }
}
