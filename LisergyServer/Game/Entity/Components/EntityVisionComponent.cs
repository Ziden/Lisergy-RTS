using Game.ECS;
using Game.World.Systems;

namespace Game.Entity.Components
{
    public class EntityVisionComponent : IComponent
    {
        public byte LineOfSight;

        static EntityVisionComponent()
        {
            SystemRegistry<EntityVisionComponent, WorldEntity>.AddSystem(new EntityVisionSystem());
        }
    }
}
