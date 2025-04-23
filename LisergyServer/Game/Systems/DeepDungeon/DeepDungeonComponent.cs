using System;
using System.Runtime.InteropServices;
using Game.Engine.ECLS;

namespace Game.Systems.Castle
{
	[StructLayout(LayoutKind.Sequential)]
	[Serializable]
	[SyncedComponent]
	public class DeepDungeonComponent : IComponent
	{
		public ushort DungeonLevel;
	}
}