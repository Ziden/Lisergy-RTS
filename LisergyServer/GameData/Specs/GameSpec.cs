using GameData.Specs;
using System;
using System.Collections.Generic;

namespace GameData
{
    [Serializable]
    public class GameSpec
    {
        public int Version;

        public ushort? InitialBuildingSpecId;
        public ushort InitialUnitSpecId;

        public BuildingSpec InitialBuilding => Buildings[InitialBuildingSpecId.Value];
        public UnitSpec InitialUnit => Units[InitialUnitSpecId];

        public GameSpec(int version)
        {
            InitialBuildingSpecId = null;
            InitialUnitSpecId = 2;
            Version = version;
            Buildings = new Dictionary<ushort, BuildingSpec>();
            Tiles = new Dictionary<ushort, TileSpec>();
            Units = new Dictionary<ushort, UnitSpec>();
            Items = new Dictionary<ushort, ItemSpec>();
            Loots = new Dictionary<ushort, LootSpec>();
            Dungeons = new Dictionary<ushort, DungeonSpec>();
            HarvestPoints = new Dictionary<byte, ResourceHarvestPointSpec>();
            Harvesting = new HarvestingSpec();
        }

        public HarvestingSpec Harvesting;
        public Dictionary<ushort, BuildingSpec> Buildings;
        public Dictionary<ushort, TileSpec> Tiles;
        public Dictionary<ushort, UnitSpec> Units;
        public Dictionary<ushort, ItemSpec> Items;
        public Dictionary<ushort, LootSpec> Loots;
        public Dictionary<ushort, DungeonSpec> Dungeons;
        public Dictionary<byte, ResourceSpec> Resources;
        public Dictionary<byte, ResourceHarvestPointSpec> HarvestPoints;
    }
}
