using GameData.Specs;
using System;
using System.Runtime.InteropServices;

namespace GameData
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct ResourceSpecId
    {
        public byte Id;
        public static implicit operator byte(ResourceSpecId d) => d.Id;
        public static implicit operator ResourceSpecId(byte b) => new ResourceSpecId() { Id = b };
        public override string ToString() => Id.ToString();
    }

    [Serializable]
    public class ResourceSpec
    {
        public ResourceSpecId SpecId;
        public ArtSpec Art;
        public string Name;
        public byte WeightPerUnit;

        public ResourceSpec(byte i)
        {
            this.SpecId = new ResourceSpecId() { Id = i };
            this.Art = default;
        }
    }
}
