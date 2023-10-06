using System;

namespace GameData.Specs
{
    public enum ArtType
    {
        SPECIFIC_SPRITE,
        SPRITE_SHEET,
        PREFAB,
    }

    [Serializable]
    public class ArtSpec
    {
        public string Address;
        public ArtType Type;
    }
}
