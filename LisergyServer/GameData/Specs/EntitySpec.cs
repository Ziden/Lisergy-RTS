using System;
using System.Runtime.InteropServices;


namespace GameData.Specs
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct EntitySpecId
    {
        public byte Id;
        public static implicit operator byte(EntitySpecId d) => d.Id;
        public static implicit operator EntitySpecId(byte b) => new EntitySpecId() { Id = b };
        public override string ToString() => Id.ToString();
    }

    public class EntitySpec
    {
        public EntitySpecId SpecId;
        public ArtSpec Icon;
    }
}
