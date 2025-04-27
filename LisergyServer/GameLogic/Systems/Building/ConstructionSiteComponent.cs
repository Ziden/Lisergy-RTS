using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Game.Engine.DataTypes;
using Game.Engine.ECLS;
using GameData;

namespace Game.Systems.Building
{
    /// <summary>
    ///     Component for buildings that are under construction
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
	[Serializable]
	[SyncedComponent]
	public class ConstructionSiteComponent : IComponent
	{
		public TimeBlock BuildingWorkPrediction;
		public BuildingSpecId BuildingSpec;

        /// <summary>
        ///     0 to 100
        /// </summary>
        public byte Percentage;

		public List<GameId> EntitiesBuilding;

		public override string ToString()
		{
			return
				$"<Construction Site {Percentage}% TimeRemaining={BuildingWorkPrediction?.TotalBlockTime.TotalSeconds ?? 0} Builders={EntitiesBuilding?.Count ?? 0}>";
		}
	}
}