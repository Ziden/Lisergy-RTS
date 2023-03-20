using System;

namespace GameData.Specs
{
    public enum ArtType
    {
        SPECIFIC_SPRITE,
        SPRITE_SHEET,
        PREFAB
    }

    [Serializable]
    public struct ArtSpec
    {
        public int Index;
        public string Name;
        public ArtType Type;
    }
}
