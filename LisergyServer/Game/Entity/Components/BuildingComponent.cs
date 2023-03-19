using Game.ECS;
using Game.Entity.Systems;

namespace Game.Entity.Components
{
    public class BuildingComponent : IComponent
    {
        static BuildingComponent()
        {
            SystemRegistry<BuildingComponent, WorldEntity>.AddSystem(new BuildingSystem());
        }
    }
}
