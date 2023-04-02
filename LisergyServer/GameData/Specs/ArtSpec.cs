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
    public struct ArtSpec
    {
        public int Index; // TODO: Remove
        public string Address;
        public ArtType Type;
    }
}
