using System;
using System.Runtime.InteropServices;
using Game.Engine.ECLS;
using Game.World;

namespace Game.Systems.Building
{
    /// <summary>
    ///     Added to entities that are building something
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
	[Serializable]
	[SyncedComponent]
	public class ConstructionWorkerComponent : IComponent
	{
		public Location BuildingAt;

		public override string ToString()
		{
			return "<Builder>";
		}
	}
}