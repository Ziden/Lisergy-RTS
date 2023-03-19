using Game.ECS;
using Game.Entity.Entities;
using Game.World.Systems;
using System.Collections.Generic;

namespace Game.World.Components
{
    public class TileVisibility : IComponent
    {
        public HashSet<PlayerEntity> PlayersViewing = new HashSet<PlayerEntity>();
        public HashSet<WorldEntity> EntitiesViewing = new HashSet<WorldEntity>();

        static TileVisibility()
        {
            SystemRegistry<TileVisibility, Tile>.AddSystem(new TileVisibilitySystem());
        }

    }
}
