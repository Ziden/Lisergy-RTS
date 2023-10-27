using System;
using System.Runtime.InteropServices;
using Game.DataTypes;

namespace Game.Systems.Resources
{
	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class ResourceRespawnComponent
	{
		public GameId RespawnTaskId;
	}
}