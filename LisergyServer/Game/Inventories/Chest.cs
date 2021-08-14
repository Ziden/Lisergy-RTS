using System;
using System.Collections.Generic;
using System.Text;

namespace Game.Inventories
{
    public class Chest : Item
    {
        private List<Item> _contents;

        public Chest(ushort specId, uint amount, List<Item> contents): base(specId, amount)
        {
            this._contents = contents;
        }

        public List<Item> GetContents()
        {
            return _contents;
        }
    }
}
