using Game.ECS;
using Game.Player;
using Game.Tile;
using System.Collections.Generic;

namespace Game.Systems.FogOfWar
{
    public class TileVisibility : IComponent
    {
        public HashSet<PlayerEntity> PlayersViewing = new HashSet<PlayerEntity>();
        public HashSet<IEntity> EntitiesViewing = new HashSet<IEntity>();
    }
}
