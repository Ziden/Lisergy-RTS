using Game.ECS;
using System.Collections.Generic;

namespace Game.World.Components
{
    public class TileVisibilityComponent : IComponent
    {
        public HashSet<PlayerEntity> PlayersViewing = new HashSet<PlayerEntity>();
        public HashSet<WorldEntity> EntitiesViewing = new HashSet<WorldEntity>();

      
    }
}
