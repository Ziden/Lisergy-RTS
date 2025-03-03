using Game.Engine.ECLS;
using Game.Systems.Tile;

namespace Game.Systems.Resources
{
    /// <summary>
    /// Logic for any entity that has a cargo component meaning he can harvest resources
    /// </summary>
    public unsafe class ResourcesLogic : BaseEntityLogic<TileDataComponent>
    {
        /// <summary>
        /// Reads the tile specs and sets up its resource components
        /// </summary>
        public void SetTileResourcesFromHarvestSpec()
        {
            var tile = Entity.Logic.Map.GetTile();
            var harvestPoint = tile.HarvestPointSpec;
            if (harvestPoint == null)
            {
                if (tile.Components.Has<TileResourceComponent>()) tile.Components.Remove<TileResourceComponent>();
                return;
            }
            if (tile.Components.Has<TileResourceComponent>()) return;

            tile.Components.Add(new TileResourceComponent());
            var res = tile.Components.Get<TileResourceComponent>();
            res.Resource = new ResourceStackData(harvestPoint.ResourceId, harvestPoint.ResourceAmount);
            tile.Save(res);
        }

    }
}