using Game.ECS;

namespace Game.FogOfWar
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
