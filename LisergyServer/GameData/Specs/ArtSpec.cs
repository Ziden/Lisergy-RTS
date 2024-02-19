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
        public string Address;
        public ArtType Type;

        public ArtSpec(string addr)
        {
            Address = addr;
            Type = ArtType.PREFAB;
        }

        public ArtSpec(string addr, ArtType type)
        {
            Address = addr;
            Type = type;
        }

        public static implicit operator string(ArtSpec d) => d.Address;
        public static implicit operator ArtSpec(string b) => new ArtSpec() { Address = b };

        public override string ToString() => $"<Art Type={Type} Addr={Address}/>";
    }
}
