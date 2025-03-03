using System;
using System.Runtime.InteropServices;

namespace GameData
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct HarvestPointSpecId
    {
        public byte Id;
        public static implicit operator byte(HarvestPointSpecId d) => d.Id;
        public static implicit operator HarvestPointSpecId(byte b) => new HarvestPointSpecId() { Id = b };
        public override string ToString() => Id.ToString();
    }

    /// <summary>
    /// Represents a harvestable resource on a given tile
    /// </summary>
    [Serializable]
    public class ResourceHarvestPointSpec
    {
        /// <summary>
        /// Spec id
        /// </summary>
        public HarvestPointSpecId SpecId;

        /// <summary>
        /// What resource to spawn
        /// </summary>
        public ResourceSpecId ResourceId;

        /// <summary>
        /// Max amount of resource to spawn
        /// </summary>
        public ushort ResourceAmount;

        /// <summary>
        /// Harvest time per 1 unit of the resource
        /// </summary>
        public TimeSpan HarvestTimePerUnit;

        /// <summary>
        /// Time to respawn every unit of the given resource after its stopped being harvested
        /// </summary>
        public TimeSpan RespawnTime;

        public ResourceHarvestPointSpec(in byte i)
        {
            SpecId = new HarvestPointSpecId() { Id = i };
        }
    }
}
