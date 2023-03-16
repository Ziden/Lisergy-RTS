using GameData.Specs;
using System;
using System.Collections.Generic;

namespace GameData
{
    // TODO: Make all immutable
    [Serializable]
    public struct GameSpec
    {
        public int Version;

        public ushort InitialBuilding;
        public ushort InitialUnit;

        public GameSpec(int version)
        {
            InitialBuilding = 0;
            InitialUnit = 2;
            Version = version;
            Buildings = new Dictionary<ushort, BuildingSpec>();
            Tiles = new Dictionary<ushort, TileSpec>();
            Units = new Dictionary<ushort, UnitSpec>();
            Items = new Dictionary<ushort, ItemSpec>();
            Loots = new Dictionary<ushort, LootSpec>();
            Dungeons = new Dictionary<ushort, DungeonSpec>();
        }

        // Goes to Client
        public Dictionary<ushort, BuildingSpec> Buildings;
        public Dictionary<ushort, TileSpec> Tiles;
        public Dictionary<ushort, UnitSpec> Units;
        public Dictionary<ushort, ItemSpec> Items;

        // Does not go to client
        [NonSerialized]
        public Dictionary<ushort, LootSpec> Loots;

        [NonSerialized]
        public Dictionary<ushort, DungeonSpec> Dungeons;
    }
}
