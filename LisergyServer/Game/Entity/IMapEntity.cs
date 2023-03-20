using Game.ECS;
using Game.Tile;

namespace Game.Entity
{
    public interface IMapEntity : IEntity
    {
        TileEntity Tile { get; set; }
    }
}
