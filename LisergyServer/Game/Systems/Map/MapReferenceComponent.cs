using Game.ECS;
using Game.Tile;

namespace Game.Systems.MapPosition
{
    /// <summary>
    /// Holds basic tile references to the entity like which tiles the entity is.
    /// This is just to speedup lookups to read which tile the entity is - so we just keep a reference "cached"
    /// </summary>
    public class MapReferenceComponent : IReferenceComponent
    {
        public TileEntity Tile;
        public TileEntity PreviousTile;

        public override string ToString()
        {
            return $"<MapReferencePositionComponent Tile={Tile}>";
        }
    }
}
