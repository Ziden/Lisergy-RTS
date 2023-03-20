using GameData.Specs;
using System.Collections.Generic;

namespace Game.Inventories
{
    public class PlayerInventories
    {
        private Dictionary<ItemType, Inventory> _inventories = new Dictionary<ItemType, Inventory>();

        public Inventory GetInventory(ItemType type)
        {
            Inventory i;
            if (!_inventories.TryGetValue(type, out i))
            {
                i = new Inventory();
                _inventories[type] = i;
            }
            return i;
        }
    }
}
