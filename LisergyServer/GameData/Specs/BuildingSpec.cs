using Game.Systems.Resources;
using GameData.Specs;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace GameData
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct BuildingSpecId
    {
        public byte Id;
        public static implicit operator byte(BuildingSpecId d) => d.Id;
        public static implicit operator BuildingSpecId(byte b) => new BuildingSpecId() { Id = b };
        public override string ToString() => Id.ToString();
    }

    public class BuildingTechTreeNode
    {
        public List<BuildingTechTreeNode> Children = new List<BuildingTechTreeNode>();

        public BuildingSpecId? UnlockBuilding;
    }

    public class BuildingTechTree
    {
        public BuildingTechTreeNode Root;
    }

    [Serializable]
    public class BuildingSpec
    {
        public BuildingSpecId SpecId;
        public ArtSpec Art;
        public byte LOS;
        public List<ResourceStackData> BuildingCost;

        public BuildingSpec(byte id)
        {
            this.SpecId = new BuildingSpecId() { Id = id };
        }
    }
}
