using GameData.Specs;
using System.Collections.Generic;

namespace Game.Systems.Inventories
{
    public class PlayerInventories
    {
        private readonly Dictionary<ItemType, Inventory> _inventories = new Dictionary<ItemType, Inventory>();

        public Inventory GetInventory(ItemType type)
        {
            if (!_inventories.TryGetValue(type, out Inventory i))
            {
                i = new Inventory();
                _inventories[type] = i;
            }
            return i;
        }
    }
}
