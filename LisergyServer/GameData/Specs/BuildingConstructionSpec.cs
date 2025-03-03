using Game.Systems.Resources;
using GameData.Specs;
using System;
using System.Collections.Generic;

namespace GameData
{
    [Serializable]
    public class BuildingTechTreeNode
    {
        public List<BuildingTechTreeNode> Children = new List<BuildingTechTreeNode>();

        public BuildingSpecId? Building;
    }

    [Serializable]
    public class ConstructionTreeSpec
    {
        public BuildingTechTreeNode Root;
    }

    [Serializable]
    public class BuildingConstructionSpec
    {
        public BuildingSpecId SpecId;
        public List<ResourceStackData> BuildingCost;
        public ArtSpec Icon;
        public ushort TimeToBuildSeconds;

        public BuildingConstructionSpec(byte id)
        {
            this.SpecId = id;
        }
    }
}
