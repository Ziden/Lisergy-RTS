using Game.Engine.DataTypes;
using Game.Engine.ECLS;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Game.Systems.Building
{
    /// <summary>
    /// Component for buildings that are under construction
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    [Serializable]
    public class ConstructionComponent : IComponent
    {
        public TimeBlock TimeBlock;

        /// <summary>
        /// 0 to 100
        /// </summary>
        public byte Percentage;

        public List<GameId> EntitiesBuilding;

        public override string ToString()
        {
            return $"<Construction {Percentage}% TimeRemaining={TimeBlock.TotalBlockTime.TotalSeconds} Builders={EntitiesBuilding.Count}>";
        }
    }
}
