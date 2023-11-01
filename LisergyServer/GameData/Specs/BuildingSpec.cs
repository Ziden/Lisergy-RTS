using GameData.Specs;
using System;
using System.Runtime.InteropServices;

namespace GameData
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct BuildingSpecId
    {
        public byte Id;
        public static implicit operator byte(BuildingSpecId d) => d.Id;
        public static implicit operator BuildingSpecId(byte b) => new BuildingSpecId() { Id = b };
        public override string ToString() => Id.ToString();
    }

    [Serializable]
    public class BuildingSpec
    {
        public BuildingSpecId SpecId;
        public ArtSpec Art;
        public byte LOS;

        public BuildingSpec(byte id, ArtSpec art, byte los = 4)
        {
            this.SpecId = new BuildingSpecId() { Id = id };
            this.Art = art;
            this.LOS = los;
        }
    }
}
