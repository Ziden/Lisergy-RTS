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
            Buildings = new Dictionary<byte, BuildingSpec>();
            BuildingConstructions = new Dictionary<byte, BuildingConstructionSpec>();
            Tiles = new Dictionary<byte, TileSpec>();
            Units = new Dictionary<byte, UnitSpec>();
            Items = new Dictionary<ushort, ItemSpec>();
            Loots = new Dictionary<ushort, LootSpec>();
            StartingResources = new Dictionary<ResourceSpecId, ushort>();
            Dungeons = new Dictionary<byte, DungeonSpec>();
            Resources = new Dictionary<byte, ResourceSpec>();
            HarvestPoints = new Dictionary<byte, ResourceHarvestPointSpec>();
            Entities = new Dictionary<int, EntitySpec>();
            Harvesting = new HarvestingSpec();
            ConstructionTechTree = new ConstructionTreeSpec();
        }

        public Dictionary<ResourceSpecId, ushort> StartingResources;
        public ConstructionTreeSpec ConstructionTechTree;
        public HarvestingSpec Harvesting;
        public Dictionary<byte, BuildingSpec> Buildings;
        public Dictionary<byte, BuildingConstructionSpec> BuildingConstructions;
        public Dictionary<byte, TileSpec> Tiles;
        public Dictionary<byte, UnitSpec> Units;
        public Dictionary<ushort, ItemSpec> Items;
        public Dictionary<ushort, LootSpec> Loots;
        public Dictionary<byte, DungeonSpec> Dungeons;
        public Dictionary<byte, ResourceSpec> Resources;
        public Dictionary<byte, ResourceHarvestPointSpec> HarvestPoints;
        public Dictionary<int, EntitySpec> Entities;
    }
}
