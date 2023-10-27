using System;

namespace GameData
{
    /// <summary>
    /// Represents a harvestable resource on a given tile
    /// </summary>
    [Serializable]
    public class ResourceHarvestPointSpec
    {
        /// <summary>
        /// Spec id
        /// </summary>
        public byte SpecId;
        
        /// <summary>
        /// What resource to spawn
        /// </summary>
        public byte ResourceId;
        
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
        
        public ResourceHarvestPointSpec(byte i)
        {
            SpecId = i;
        }
    }
}
