using GameData.Specs;
using System;
using System.Collections.Generic;

namespace GameData
{
    // TODO: Make all immutable
    [Serializable]
    public class GameSpec
    {
        public int Version = 1;

        public ushort InitialBuilding = 0;
        public ushort InitialUnit = 2;

        // Goes to Client
        public Dictionary<ushort, BuildingSpec> Buildings = new Dictionary<ushort, BuildingSpec>();
        public Dictionary<ushort, TileSpec> Tiles = new Dictionary<ushort, TileSpec>();
        public Dictionary<ushort, UnitSpec> Units = new Dictionary<ushort, UnitSpec>();
        public Dictionary<ushort, ItemSpec> Items = new Dictionary<ushort, ItemSpec>();

        // Does not go to client
        [NonSerialized]
        public Dictionary<ushort, LootSpec> Loots = new Dictionary<ushort, LootSpec>();

        [NonSerialized]
        public Dictionary<ushort, DungeonSpec> Dungeons = new Dictionary<ushort, DungeonSpec>();
    }
}
