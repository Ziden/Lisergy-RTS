using System;
using System.Collections.Generic;
using Game.Engine.Scheduler;
using GameData;

namespace Game.Systems.Resources
{
	public class ResourceRespawn
	{
		public TileSpecId ChangeToTileSpecId;
		public ResourceStackData Resource;
		public DateTime RespawnTime;
	}

	[Serializable]
	public class RespawnTaskExecutor : ITaskExecutor
	{
		private SortedSet<ResourceRespawn> _respawnQueue = new SortedSet<ResourceRespawn>();

		public void Execute(GameTask task)
		{
			// _respawnQueue.
		}
	}
}