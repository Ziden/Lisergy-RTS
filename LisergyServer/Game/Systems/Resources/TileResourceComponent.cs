using System;
using System.Runtime.InteropServices;
using Game.ECS;
using GameData;

namespace Game.Systems.Resources
{
	/// <summary>
	/// Represents a resource harvest point that can be harvested
	/// This component is added to tiles
	/// </summary>
	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	[SyncedComponent]
	[NonPersisted]
	public struct TileResourceComponent : IComponent
	{
		/// <summary>
		/// Amount of resources left
		/// </summary>
		public ushort AmountResourcesLeft;

		/// <summary>
		/// The resource to be harvested
		/// </summary>
		public ResourceSpecId ResourceId;

		/// <summary>
		/// If this tile is currently being harvested or not
		/// </summary>
		public bool BeingHarvested;
	}
}