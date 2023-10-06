using Game.ECS;
using Game.Tile;

namespace Game.Systems.MapPosition
{
    public class MapReferenceComponent : IServerComponent
    {
        public TileEntity Tile;
        public TileEntity PreviousTile;

        public override string ToString()
        {
            return $"<MapReferencePositionComponent Tile={Tile}>";
        }
    }
}
