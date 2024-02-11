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
            var harvest = GetAvailableResourcesToHarvest(tile);
            if (Entity.Components.Has<HarvestingComponent>()) return false;
            if (harvest.Amount <= 0) return false;
            if (Entity.Get<CargoComponent>().GetRoomFor(harvest.ResourceId) == -1) return false;
            var pos = Entity.Get<MapPlacementComponent>();
            return pos.Position.Distance(tile.Position) <= 1;
        }

        /// <summary>
        /// Gets the available resources to be harvested on a given tile
        /// </summary>
        public ResourceStackData GetAvailableResourcesToHarvest(TileEntity tile)
        {
            if (!tile.Components.TryGet<TileResourceComponent>(out var harvest))
            {
                return default;
            }
            else return harvest.Resource;
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
            Game.Log.Debug($"{Entity} starting harvesting on {tile}");
            var tileResources = tile.Components.GetPointer<TileResourceComponent>();
            tileResources->BeingHarvested = true;
            Entity.Components.Add<HarvestingComponent>();
            var harvesting = Entity.Components.GetPointer<HarvestingComponent>();
            harvesting->Tile = tile.Position;
            harvesting->StartedAt = Game.GameTime.ToBinary();
            var ev = EventPool<HarvestingStartedEvent>.Get();
            ev.Tile = tile;
            ev.Harvester = Entity;
            ev.Resource = tileResources->Resource.ResourceId;
            Entity.Components.CallEvent(ev);
            EventPool<HarvestingStartedEvent>.Return(ev);
            return true;
        }

        /// <summary>
        /// Gets the amount of resources harvested so far
        /// </summary>
        /// <returns></returns>
        public ushort GetAmountHarvestedCurrently()
        {
            var harvesting = Entity.Components.Get<HarvestingComponent>();
            var tile = Game.World.Map.GetTile(harvesting.Tile.X, harvesting.Tile.Y);
            var totalTimeHarvesting = Game.GameTime - DateTime.FromBinary(harvesting.StartedAt);
            var tileSpec = Game.Specs.Tiles[tile.SpecId];
            if (!tileSpec.ResourceSpotSpecId.HasValue)
            {
                return default;
            }
            var harvestSpec = Game.Specs.HarvestPoints[tileSpec.ResourceSpotSpecId.Value];
            var amountHarvested = (ushort)Math.Floor(totalTimeHarvesting / harvestSpec.HarvestTimePerUnit);
            return amountHarvested;
        }

        /// <summary>
        /// Stops harvesting the resource the entity is harvesting.
        /// The amount of resources collected will be calculated at this time
        /// </summary>
        public ResourceStackData StopHarvesting()
        {
            var cargo = Entity.Components.Get<CargoComponent>();
            var harvesting = Entity.Components.Get<HarvestingComponent>();
            var tile = Game.World.Map.GetTile(harvesting.Tile.X, harvesting.Tile.Y);

            var amountHarvested = GetAmountHarvestedCurrently();
            Entity.Components.Remove<HarvestingComponent>();

            var tileResources = tile.Components.GetPointer<TileResourceComponent>();
            var resourceSpec = Game.Specs.Resources[tileResources->Resource.ResourceId];

            var unitsCanCarry = (ushort)(cargo.RemainingWeight / resourceSpec.WeightPerUnit);
            if (amountHarvested > tileResources->Resource.Amount)
                amountHarvested = tileResources->Resource.Amount;
            if (amountHarvested > unitsCanCarry)
                amountHarvested = unitsCanCarry;
            tileResources->BeingHarvested = false;
            tileResources->Resource.Amount = (ushort)(tileResources->Resource.Amount - amountHarvested);
            var finalStack = new ResourceStackData(tileResources->Resource.ResourceId, amountHarvested);
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