using System;
using System.Collections.Generic;
using System.Text;

namespace GameData.Specs
{
    [Serializable]
    public class ItemSpec
    {
        public ushort Id;
        public ArtSpec Art;
        public ItemType Type;
        public string Name;
    }
}
