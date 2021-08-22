using System;

namespace GameData
{
    [Serializable]
    public class BuildingSpec
    {
        public ushort Id;
        public int ModelID;
        public byte LOS;

        public BuildingSpec(ushort id, int model, byte los=4)
        {
            this.Id = id;
            this.ModelID = model;
            this.LOS = los;
        }
    }
}
