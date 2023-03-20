using Game.ECS;

namespace Game.Building
{
    public class BuildingComponent : IComponent
    {
        static BuildingComponent()
        {
            SystemRegistry<BuildingComponent, WorldEntity>.AddSystem(new BuildingSystem());
        }
    }
}
