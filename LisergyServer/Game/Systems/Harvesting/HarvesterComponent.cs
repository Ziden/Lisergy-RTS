using System;
using System.Runtime.InteropServices;
using Game.ECS;
using Game.World;

namespace Game.Systems.Resources
{
	/// <summary>
	/// Component added to entities that can harvest resources
	/// </summary>
	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	[RequiresComponent(typeof(CargoComponent))]
	public struct HarvesterComponent : IComponent {}
}