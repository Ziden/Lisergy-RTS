using System;

namespace GameData
{
    [Serializable]
    public class BuildingSpec
    {
        public byte Id;
        public int ModelID;
        public byte LOS = 4;
    }
}
