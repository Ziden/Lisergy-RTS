using Game.ECS;

namespace Game.Entity
{
    public interface IMapEntity : IEntity
    {
        Tile Tile { get; set; }
    }
}
