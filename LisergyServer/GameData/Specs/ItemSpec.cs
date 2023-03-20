using System;

namespace GameData.Specs
{
    [Serializable]
    public struct ItemSpec
    {
        public ushort Id;
        public ArtSpec Art;
        public ItemType Type;
        public string Name;
    }
}
