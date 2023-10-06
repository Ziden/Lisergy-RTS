using GameData.Specs;
using System;

namespace GameData
{
    [Serializable]
    public class BuildingSpec
    {
        public ushort Id;
        public ArtSpec Art;
        public byte LOS;

        public BuildingSpec(ushort id, ArtSpec art, byte los = 4)
        {
            this.Id = id;
            this.Art = art;
            this.LOS = los;
        }
    }
}
