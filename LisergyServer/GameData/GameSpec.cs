using System;
using System.Collections.Generic;
using GameData.Specs;

namespace GameData;

[Serializable]
public class GameSpec
{
	public ConstructionTreeSpec ConstructionTechTree;
	public HarvestingSpec Harvesting;
	public UnitSpecId InitialUnitSpecId;
	public int Version;
	public Dictionary<byte, BuildingConstructionSpec> BuildingConstructions;
	public Dictionary<byte, BuildingSpec> Buildings;
	public Dictionary<byte, DungeonSpec> Dungeons;
	public Dictionary<int, EntitySpec> Entities;
	public Dictionary<byte, ResourceHarvestPointSpec> HarvestPoints;

	public BuildingSpecId? InitialBuildingSpecId;
	public Dictionary<ushort, ItemSpec> Items;
	public Dictionary<ushort, LootSpec> Loots;
	public Dictionary<byte, ResourceSpec> Resources;

	public Dictionary<ResourceSpecId, ushort> StartingResources;
	public Dictionary<byte, TileSpec> Tiles;
	public Dictionary<byte, UnitSpec> Units;

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

	public BuildingSpec InitialBuilding => Buildings[InitialBuildingSpecId.Value];
	public UnitSpec InitialUnit => Units[InitialUnitSpecId];
}