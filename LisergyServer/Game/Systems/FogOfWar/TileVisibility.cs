using Game.ECS;
using Game.Systems.Player;
using System.Collections.Generic;

namespace Game.Systems.FogOfWar
{
    public class TileVisibility : IComponent
    {
        public HashSet<PlayerEntity> PlayersViewing { get; internal set; } = new HashSet<PlayerEntity>();
        public HashSet<IEntity> EntitiesViewing { get; internal set; } = new HashSet<IEntity>();
    }
}
