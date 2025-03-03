using System;

namespace GameData.Specs
{
    [Serializable]
    public class EntitySpec
    {
        public string Name;
        public int Type;
        public ArtSpec Icon;
        public byte[] Components;
    }
}
