using System;
using System.Collections.Generic;

namespace Game.Inventories
{
    [Serializable]
    public class Inventory : Dictionary<ushort, Item>
    {
        public void AddItem(Item i)
        {
            if (!TryGetValue(i.SpecID, out Item alreadyhave))
            {
                this[i.SpecID] = i;
            }
            else
            {
                alreadyhave.Amount += i.Amount;
            }
        }
    }
}