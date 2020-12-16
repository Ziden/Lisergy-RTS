using System;
using System.Collections.Generic;
using System.Text;

namespace GameData.Specs
{
    public enum ArtType
    {
        SPRITE,
        PREFAB
    }

    [Serializable]
    public class ArtSpec
    {
        public string Name;
        public ArtType Type;
    }
}
