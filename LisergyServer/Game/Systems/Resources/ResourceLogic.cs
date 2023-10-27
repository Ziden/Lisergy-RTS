using System;
using Game.ECS;
using Game.Systems.Map;
using Game.Tile;
using Game.World;

namespace Game.Systems.Resources
{
	/// <summary>
	/// Logic for any entity that has a cargo component meaning he can harvest resources
	/// </summary>
	public unsafe class HarvestingLogic : BaseEntityLogic<CargoComponent>
	{
		/// <summary>
		/// Checks if the current entity can harvest the given tile
		/// </summary>
		public bool CanHarvest(TileEntity tile)
		{
			if (tile.Components.TryGet<TileResourceComponent>(out var harvest))
			{
				if (harvest.AmountResourcesLeft == 0) return false;
				if (harvest.BeingHarvested) return false;
			}
			else return false;
			if (!Entity.Get<CargoComponent>().HasRoom) return false;
			var pos = Entity.Get<MapPlacementComponent>();
			return pos.Position.Distance(tile.Position) <= 1;
		}

		/// <summary>
		/// Starts harvesting the given tile.
		/// </summary>
		public void StartHarvesting(TileEntity tile)
		{
			if (!CanHarvest(tile)) return;
			var tileResources = tile.Components.GetPointer<TileResourceComponent>();
			tileResources->BeingHarvested = true;
			var harvesting = Entity.Components.GetPointer<HarvestingComponent>();
			harvesting->Tile = tile.Position;
			harvesting->StartedAt = Game.GameTime;
		}

		public CargoResource StopHarvesting()
		{
			var cargo = Entity.Components.Get<CargoComponent>();
			var harvesting = Entity.Components.GetPointer<HarvestingComponent>();
			var tile = Game.World.Map.GetTile(harvesting->Tile.X, harvesting->Tile.Y);
			var tileSpec = Game.Specs.Tiles[tile.SpecId];
			if (!tileSpec.ResourceSpotSpecId.HasValue)
			{
				throw new Exception($"Error stopping harvesting. Tile {tile} have no spot setup");
			}
			var harvestSpec = Game.Specs.HarvestPoints[tileSpec.ResourceSpotSpecId.Value];
			var tileResources = tile.Components.GetPointer<TileResourceComponent>();
			var resourceSpec = Game.Specs.Resources[tileResources->ResourceId];
			var totalTimeHarvesting = harvesting->StartedAt - Game.GameTime;
			var amountHarvested = (ushort)Math.Floor(totalTimeHarvesting / harvestSpec.HarvestTimePerUnit);
			var remainingWeight = cargo.RemainingWeight;
			var unitsCanCarry = (ushort)(remainingWeight / resourceSpec.WeightPerUnit);
			if (amountHarvested > tileResources->AmountResourcesLeft)
				amountHarvested = tileResources->AmountResourcesLeft;
			if (amountHarvested > unitsCanCarry)
				amountHarvested = unitsCanCarry;
			tileResources->BeingHarvested = false;
			tileResources->AmountResourcesLeft = (ushort)(tileResources->AmountResourcesLeft - amountHarvested);
			return new CargoResource()
			{
				Amount = amountHarvested,
				ResourceSpecId = tileResources->ResourceId
			};
		}
	}
}