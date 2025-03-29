using Game.Systems.Resources;
using GameData.Specs;
using System;
using System.Collections.Generic;

namespace GameData
{
    [Serializable]
    public class ConstructionTreeSpec
    {
        public NodeTree<BuildingSpecId> Root;
    }

    [Serializable]
    public class BuildingConstructionSpec
    {
        public BuildingSpecId SpecId;
        public List<ResourceStackData> Costs;
        public ArtSpec Icon;
        public ushort TimeToBuildSeconds;

        public BuildingConstructionSpec(byte id)
        {
            this.SpecId = id;
        }
    }
}
