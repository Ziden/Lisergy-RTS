using Game.ECS;
using Game.World.Systems;

namespace Game.Entity.Components
{
    public class EntityExplorationComponent : IComponent
    {
        public byte LineOfSight;

        static EntityExplorationComponent()
        {
            SystemRegistry<EntityExplorationComponent, WorldEntity>.AddSystem(new EntityExplorationSystem());
        }
    }
}
