using Game.ECS;
using Game.Systems.Player;
using System.Collections.Generic;

namespace Game.Systems.FogOfWar
{
    /// <summary>
    /// Keeps track of who is viewing this tile for fast reference accessing
    /// </summary>
    public class TileVisibility : IReferenceComponent
    {
        public HashSet<PlayerEntity> PlayersViewing { get; internal set; } = new HashSet<PlayerEntity>();
        public HashSet<IEntity> EntitiesViewing { get; internal set; } = new HashSet<IEntity>();

        public override string ToString()
        {
            return $"<TileVisibility PlayersViewing={PlayersViewing.Count} EntitiesViewing={EntitiesViewing.Count}>";
        }
    }
}
