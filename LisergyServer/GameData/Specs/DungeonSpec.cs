using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace GameData.Specs
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct DungeonSpecId
    {
        public byte Id;
        public static implicit operator byte(DungeonSpecId d) => d.Id;
        public static implicit operator DungeonSpecId(byte b) => new DungeonSpecId() { Id = b };
    }

    [Serializable]
    public class DungeonSpec
    {
        public DungeonSpecId SpecId;
        public ArtSpec Art;
        public List<BattleSpec> BattleSpecs;
        public ushort LootSpecID;
    }
}
