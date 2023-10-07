using Game.ECS;

namespace Game.Systems.Building
{
    public struct BuildingComponent : IComponent
    {
        public override string ToString()
        {
            return $"<BuildingComponent>";
        }
    }
}
