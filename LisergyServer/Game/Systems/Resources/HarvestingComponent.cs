using System;
using System.Runtime.InteropServices;
using Game.ECS;
using Game.World;

namespace Game.Systems.Resources
{
	/// <summary>
	/// Component to be placed on entities while they are harvesting
	/// </summary>
	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	[SyncedComponent]
	public struct HarvestingComponent : IComponent
	{
		public Position Tile;
		public DateTime StartedAt;
	}
}