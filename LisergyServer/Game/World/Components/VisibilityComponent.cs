using Game.ECS;
using Game.World.Systems;
using System.Collections.Generic;

namespace Game.World.Components
{
    public class TileVisibilityComponent : IComponent
    {
        public HashSet<PlayerEntity> PlayersViewing = new HashSet<PlayerEntity>();
        public HashSet<WorldEntity> EntitiesViewing = new HashSet<WorldEntity>();

        static TileVisibilityComponent()
        {
            SystemRegistry<TileVisibilityComponent, Tile>.AddSystem(new TileVisibilitySystem());
        }

    }
}
