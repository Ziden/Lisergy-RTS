using System;
using Game.ECS;
using Game.Events;
using Game.Systems.Harvesting;
using Game.Systems.Map;
using Game.Tile;
using Game.World;

namespace Game.Systems.Resources
{
    /// <summary>
    /// Logic for any entity that has a cargo component meaning he can harvest resources
    /// </summary>
    public unsafe class HarvestingLogic : BaseEntityLogic<HarvesterComponent>
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
			if (Entity.Components.Has<HarvestingComponent>()) return false;
			if (Entity.Get<CargoComponent>().GetRoomFor(harvest.ResourceId) == -1) return false;
			var pos = Entity.Get<MapPlacementComponent>();
			return pos.Position.Distance(tile.Position) <= 1;
		}

		/// <summary>
		/// Returns if given entity is currently harvesting
		/// </summary>
		public bool IsHarvesting() => Entity.Components.Has<HarvestingComponent>();

		/// <summary>
		/// Starts harvesting the given tile.
		/// </summary>
		public bool StartHarvesting(TileEntity tile)
		{
			if (!CanHarvest(tile)) return false;
			var tileResources = tile.Components.GetPointer<TileResourceComponent>();
			tileResources->BeingHarvested = true;
			Entity.Components.Add<HarvestingComponent>();
			var harvesting = Entity.Components.GetPointer<HarvestingComponent>();
			harvesting->Tile = tile.Position;
			harvesting->StartedAt = Game.GameTime;
			var ev = EventPool<HarvestingStartedEvent>.Get();
			ev.Tile = tile;
			ev.Harvester = Entity;
			ev.Resource = tileResources->ResourceId;
			Entity.Components.CallEvent(ev);
			EventPool<HarvestingStartedEvent>.Return(ev);
			Game.Log.Debug($"{Entity} started harvesting on {tile}");
			return true;
		}

		/// <summary>
		/// Stops harvesting the resource the entity is harvesting.
		/// The amount of resources collected will be calculated at this time
		/// </summary>
        public ResourceStackData StopHarvesting()
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
			var totalTimeHarvesting = Game.GameTime - harvesting->StartedAt;
			var amountHarvested = (ushort)Math.Floor(totalTimeHarvesting / harvestSpec.HarvestTimePerUnit);
			var remainingWeight = cargo.RemainingWeight;
			var unitsCanCarry = (ushort)(remainingWeight / resourceSpec.WeightPerUnit);
			if (amountHarvested > tileResources->AmountResourcesLeft)
				amountHarvested = tileResources->AmountResourcesLeft;
			if (amountHarvested > unitsCanCarry)
				amountHarvested = unitsCanCarry;
			tileResources->BeingHarvested = false;
			tileResources->AmountResourcesLeft = (ushort)(tileResources->AmountResourcesLeft - amountHarvested);
			Entity.Components.Remove<HarvestingComponent>();
			var finalStack = new ResourceStackData(tileResources->ResourceId, amountHarvested);
            var ev = EventPool<HarvestingEndedEvent>.Get();
            ev.Tile = tile;
            ev.Harvester = Entity;
			ev.Resource = finalStack;
            Entity.Components.CallEvent(ev);
            EventPool<HarvestingEndedEvent>.Return(ev);
            Game.Log.Debug($"{Entity} finished harvesting on {tile} and got {amountHarvested}x{resourceSpec.Name}");
			return finalStack;

        }
	}
}