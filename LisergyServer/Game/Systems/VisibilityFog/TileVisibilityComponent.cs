using Game.Engine.DataTypes;
using Game.Engine.ECLS;
using System;
using System.Collections.Generic;

namespace Game.Systems.FogOfWar
{
    /// <summary>
    /// Keeps track of who is viewing this tile for fast reference accessing
    /// </summary>
    [Serializable]
    public class TileVisibilityComponent : IComponent
    {
        // TODO: Use HashSet !!!!!!
        public HashSet<GameId> PlayersViewing { get; internal set; } = new HashSet<GameId>();
        public HashSet<GameId> EntitiesViewing { get; internal set; } = new HashSet<GameId>();

        public override string ToString()
        {
            return $"<TileVisibility PlayersViewing={PlayersViewing.Count} EntitiesViewing={EntitiesViewing.Count}>";
        }
    }
}
