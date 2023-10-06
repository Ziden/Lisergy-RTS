using Game.ECS;
using Game.Systems.Player;
using System.Collections.Generic;

namespace Game.Systems.FogOfWar
{
    public class TileVisibility : IComponent
    {
        public HashSet<Player.PlayerEntity> PlayersViewing { get; internal set; } = new HashSet<Player.PlayerEntity>();
        public HashSet<IEntity> EntitiesViewing { get; internal set; } = new HashSet<IEntity>();

        public override string ToString()
        {
            return $"<TileVisibility PlayersViewing={PlayersViewing.Count} EntitiesViewing={EntitiesViewing.Count}>";
        }
    }
}
