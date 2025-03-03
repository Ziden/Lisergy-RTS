﻿using Game.Engine.ECLS;
using Game.Engine.Events;
using Game.Systems.Resources;
using Game.Tile;
using GameData;

namespace Game.Systems.Harvesting
{
    /// <summary>
    /// Triggered when entity starts harvesting from a tile
    /// </summary>
    public class HarvestingStartedEvent : IGameEvent
    {
        public IEntity Harvester;
        public TileModel Tile;
        public ResourceSpecId Resource;
    }

    /// <summary>
    /// Triggered when entity finishes harvesting from a tile
    /// </summary>
    public class HarvestingEndedEvent : IGameEvent
    {
        public IEntity Harvester;
        public TileModel Tile;
        public ResourceStackData Resource;
    }
}
