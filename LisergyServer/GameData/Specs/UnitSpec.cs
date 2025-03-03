using Game.Specs;
using System;
using System.Runtime.InteropServices;

namespace GameData.Specs
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct UnitSpecId
    {
        public byte Id;
        public static implicit operator byte(UnitSpecId d) => d.Id;
        public static implicit operator UnitSpecId(byte b) => new UnitSpecId() { Id = b };
        public override string ToString() => Id.ToString();
    }

    [Serializable]
    public class UnitSpec
    {
        public string Name;
        public UnitSpecId SpecId;
        public byte LOS;
        public ArtSpec Art;
        public ArtSpec IconArt;
        public UnitStats Stats;
    }
}
