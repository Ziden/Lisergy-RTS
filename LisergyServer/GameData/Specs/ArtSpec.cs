using System;
using System.Collections.Generic;
using System.Text;

namespace GameData.Specs
{
    public enum ArtType
    {
        SPECIFIC_SPRITE,
        SPRITE_SHEET,
        PREFAB
    }

    [Serializable]
    public class ArtSpec
    {
        public int Index;
        public string Name;
        public ArtType Type;
    }
}
