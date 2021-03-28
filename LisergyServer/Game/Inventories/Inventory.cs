using System;
using System.Collections.Generic;

namespace Game.Inventories
{
    [Serializable]
    public class Inventory
    {
        private Dictionary<ushort, Item> _items = new Dictionary<ushort, Item>();

        [NonSerialized]
        private readonly int _defaultMaxStack = 100;

        [NonSerialized]
        private Dictionary<ushort, int> _maxStacks = new Dictionary<ushort, int>();
    }
}
