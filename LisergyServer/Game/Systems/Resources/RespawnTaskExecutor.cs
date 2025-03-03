using Game.Engine.Scheduler;
using GameData;
using System;
using System.Collections.Generic;

namespace Game.Systems.Resources
{
    public class ResourceRespawn
    {
        public DateTime RespawnTime;
        public TileSpecId ChangeToTileSpecId;
        public ResourceStackData Resource;
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
