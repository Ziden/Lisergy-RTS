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
            this.SpecID = specId;
            this.Amount = amount;
        }
      
    }
}
