using Game.ECS;
using Game.World.Systems;
using System.Collections.Generic;

namespace Game.World.Components
{
    public class EntityPlacementComponent : IComponent
    {
        public List<WorldEntity> EntitiesIn = new List<WorldEntity>();
        public StaticEntity StaticEntity;

        static EntityPlacementComponent()
        {
            SystemRegistry<EntityPlacementComponent, Tile>.AddSystem(new EntityPlacementSystem());
        }
    }
}
