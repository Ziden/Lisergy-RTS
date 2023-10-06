using GameData.Specs;
using System;

namespace Game.Systems.Inventories
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
    }
}
