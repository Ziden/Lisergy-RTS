using Game.ECS;
using Game.Systems.Tile;
using Game.Tile;

namespace Game.Systems.Resources
{
	/// <summary>
	/// Logic for any entity that has a cargo component meaning he can harvest resources
	/// </summary>
	public unsafe class ResourcesLogic : BaseEntityLogic<TileComponent>
	{
        /// <summary>
        /// Reads the tile specs and sets up its resource components
        /// </summary>
        public void SetTileResourcesFromHarvestSpec()
        {
            var tile = (TileEntity)Entity;
            var harvestPoint = tile.HarvestPointSpec;
            if (harvestPoint == null)
            {
                if(tile.Components.Has<TileResourceComponent>()) tile.Components.Remove<TileResourceComponent>();
                return;
            }
            tile.Components.Add<TileResourceComponent>();
            var res = tile.Components.GetPointer<TileResourceComponent>();
            res->AmountResourcesLeft = harvestPoint.ResourceAmount;
            res->ResourceId = harvestPoint.ResourceId;
        }

    }
}