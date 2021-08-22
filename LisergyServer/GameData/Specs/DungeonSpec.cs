using System;
using System.Collections.Generic;

namespace GameData.Specs
{
    [Serializable]
    public struct DungeonSpec
    {
        public ushort DungeonSpecID;
        public ArtSpec Art;
        public List<BattleSpec> BattlesUnitSpecIds;
        public ushort LootSpecID;
    }
}
