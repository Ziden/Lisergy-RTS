using GameData.Specs;
using System;
using System.Collections.Generic;

namespace GameData
{
    [Serializable]
    public class GameSpec
    {
        public int Version;

        public BuildingSpecId? InitialBuildingSpecId;
        public UnitSpecId InitialUnitSpecId;

        public BuildingSpec InitialBuilding => Buildings[InitialBuildingSpecId.Value];
        public UnitSpec InitialUnit => Units[InitialUnitSpecId];

        public GameSpec(int version)
        {
            InitialBuildingSpecId = null;
            InitialUnitSpecId = 2;
            Version = version;
            Buildings = new Dictionary<BuildingSpecId, BuildingSpec>();
            Tiles = new Dictionary<TileSpecId, TileSpec>();
            Units = new Dictionary<UnitSpecId, UnitSpec>();
            Items = new Dictionary<ushort, ItemSpec>();
            Loots = new Dictionary<ushort, LootSpec>();
            Dungeons = new Dictionary<DungeonSpecId, DungeonSpec>();
            Resources = new Dictionary<ResourceSpecId, ResourceSpec>();
            HarvestPoints = new Dictionary<HarvestPointSpecId, ResourceHarvestPointSpec>();
            Harvesting = new HarvestingSpec();
        }

        public HarvestingSpec Harvesting;
        public Dictionary<BuildingSpecId, BuildingSpec> Buildings;
        public Dictionary<TileSpecId, TileSpec> Tiles;
        public Dictionary<UnitSpecId, UnitSpec> Units;
        public Dictionary<ushort, ItemSpec> Items;
        public Dictionary<ushort, LootSpec> Loots;
        public Dictionary<DungeonSpecId, DungeonSpec> Dungeons;
        public Dictionary<ResourceSpecId, ResourceSpec> Resources;
        public Dictionary<HarvestPointSpecId, ResourceHarvestPointSpec> HarvestPoints;
    }
}
