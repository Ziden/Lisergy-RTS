using Game.ECS;

namespace Game.Systems.Building
{
    /// <summary>
    /// Represents something that can be placed statically in the map
    /// Only one static can be in a given tile at a time.
    /// </summary>
    public struct BuildingComponent : IComponent
    {
        public override string ToString()
        {
            return $"<BuildingComponent>";
        }
    }
}
