using GameData.Specs;
using System;
using System.Collections.Generic;

namespace GameData
{
    [Serializable]
    public class ResourceSpec
    {
        public byte SpecId;
        public ArtSpec Art;
        public string Name;
        public byte WeightPerUnit;

        public ResourceSpec(byte i)
        {
            this.SpecId = i;
            this.Art = default;
        }
    }
}
