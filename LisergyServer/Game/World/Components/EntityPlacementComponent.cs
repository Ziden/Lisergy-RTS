using Game.Battle;
using Game.ECS;
using Game.Entity;
using Game.Events.Bus;
using Game.Events.GameEvents;
using Game.Events.ServerEvents;
using Game.Movement;
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
