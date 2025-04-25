using System;
using System.Collections.Generic;
using Game.Systems.Resources;
using GameData.Specs;

namespace GameData;

[Serializable]
public class ConstructionTreeSpec
{
	public NodeTree<BuildingSpecId> Root;
}

[Serializable]
public class BuildingConstructionSpec
{
	public List<ResourceStackData> Costs;
	public ArtSpec Icon;
	public BuildingSpecId SpecId;
	public BuildingSpecId ConstructionSiteSpec;
	public ushort TimeToBuildSeconds;

	public BuildingConstructionSpec(byte id)
	{
		SpecId = id;
	}
}