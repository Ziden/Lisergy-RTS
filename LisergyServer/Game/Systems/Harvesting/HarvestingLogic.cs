using Game.Engine.DataTypes;
using Game.Engine.ECLS;
using Game.Engine.Events;
using Game.Systems.Harvesting;
using Game.Systems.Map;
using Game.Tile;
using Game.World;
using System;

namespace Game.Systems.Resources
{
    /// <summary>
    /// Small predictive snapshot of the current harvesting state
    /// </summary>
    public struct HarvestingTaskState
    {
        /// <summary>
        /// Harvesting task prediction data
        /// </summary>
        public TimeBlockSnapshot TimeSnapshot;

        /// <summary>
        /// What are the resources and amount being harvested
        /// </summary>
        public TileResourceComponent Resources;

        /// <summary>
        /// How many resource was already harvested
        /// </summary>
        public ushort AmountHarvested;

        /// <summary>
        /// How many units of the resource still fits the cargo ?
        /// </summary>
        public ushort CargoAvailableForUnits;
    }

    /// <summary>
    /// Logic for any entity that has a cargo component meaning he can harvest resources
    /// </summary>
    public unsafe class HarvestingLogic : BaseEntityLogic<HarvesterComponent>
    {
        /// <summary>
        /// Checks if the current entity can harvest the given tile
        /// </summary>
        public bool CanHarvest(TileModel tile)
        {
            var harvest = GetAvailableResourcesToHarvest(tile);
            if (CurrentEntity.Components.Has<HarvestingComponent>()) return false;
            if (harvest.Amount <= 0) return false;
            var cargo = CurrentEntity.Get<CargoComponent>();
            if (cargo.GetRoomFor(harvest.ResourceId) == -1) return false;
            var resourceSpec = Game.Specs.Resources[harvest.ResourceId];
            if (cargo.RemainingWeight < resourceSpec.WeightPerUnit) return false;
            var pos = CurrentEntity.Get<MapPlacementComponent>();
            return pos.Position.Distance(tile.Position) <= 1;
        }

        /// <summary>
        /// Gets the available resources to be harvested on a given tile
        /// </summary>
        public ResourceStackData GetAvailableResourcesToHarvest(TileModel tile)
        {
            if (!tile.Components.TryGet<TileResourceComponent>(out var harvest))
            {
                return default;
            }
            else return harvest.Resource;
        }

        public bool HasHarvestingResources()
        {
            return CurrentEntity.Components.Has<TileResourceComponent>();
        }


        /// <summary>
        /// Returns if given entity is currently harvesting
        /// </summary>
        public bool IsHarvesting() => CurrentEntity.Components.Has<HarvestingComponent>();

        /// <summary>
        /// Starts harvesting the given tile.
        /// </summary>
        public bool StartHarvesting(TileModel tile)
        {
            if (!CanHarvest(tile)) return false;
            Game.Log.Debug($"{CurrentEntity} starting harvesting on {tile}");

            var tileResources = tile.Components.Get<TileResourceComponent>();
            tileResources.BeingHarvested = true;
            tile.Save(tileResources);

            CurrentEntity.Components.Add<HarvestingComponent>();

            var harvesting = CurrentEntity.Components.Get<HarvestingComponent>();
            harvesting.Tile = tile.Position;
            harvesting.StartedAt = Game.GameTime.ToBinary();
            CurrentEntity.Save(harvesting);
            var ev = EventPool<HarvestingStartedEvent>.Get();
            ev.Tile = tile;
            ev.Harvester = CurrentEntity;
            ev.Resource = tileResources.Resource.ResourceId;
            CurrentEntity.Components.CallEvent(ev);
            EventPool<HarvestingStartedEvent>.Return(ev);
            return true;
        }

        /// <summary>
        /// Given the current entity max cargo and resources on the given tile
        /// Gets the amount of resource that could be harvested from a given tile
        /// </summary>
        public (TimeBlock time, ResourceStackData resource)? GetPossibleHarvest(TileModel tile)
        {
            var tileSpec = Game.Specs.Tiles[tile.SpecId];
            if (!tileSpec.ResourceSpotSpecId.HasValue)
            {
                return default;
            }
            var harvestSpec = Game.Specs.HarvestPoints[tileSpec.ResourceSpotSpecId.Value];
            var tileResources = tile.Components.Get<TileResourceComponent>();
            var toHarvest = tileResources.Resource;
            CurrentEntity.Logic.Cargo.TrimResourcesToMaxCargo(ref toHarvest);
            var timeToHarvest = toHarvest.Amount * harvestSpec.HarvestTimePerUnit;
            var block = new TimeBlock()
            {
                StartTime = Game.GameTime,
                EndTime = Game.GameTime + timeToHarvest
            };
            return (block, toHarvest);
        }

        /// <summary>
        /// Calculates what's the current state of the harvesting task.
        /// </summary>
        public HarvestingTaskState CalculateCurrentState()
        {
            var harvesting = CurrentEntity.Components.Get<HarvestingComponent>();
            var tile = Game.World.GetTile(harvesting.Tile.X, harvesting.Tile.Y);
            var startTime = DateTime.FromBinary(harvesting.StartedAt);

            //
            var tileSpec = Game.Specs.Tiles[tile.SpecId];
            if (!tileSpec.ResourceSpotSpecId.HasValue)
            {
                return default;
            }
            var harvestSpec = Game.Specs.HarvestPoints[tileSpec.ResourceSpotSpecId.Value];
            var tileResources = tile.Components.Get<TileResourceComponent>();
            var totalTaskTime = (harvestSpec.HarvestTimePerUnit * tileResources.Resource.Amount);
            var timeBlock = startTime.GetTimeBlock(totalTaskTime);
            var timeSnapshot = timeBlock.GetCurrentSnapshot(Game.GameTime);
            var possible = GetPossibleHarvest(tile);
            var harvested = possible.Value.resource;
            harvested.Amount = (ushort)Math.Round(harvested.Amount * timeSnapshot.Percentagage);
            return new HarvestingTaskState()
            {
                AmountHarvested = harvested.Amount,
                Resources = tileResources,
                TimeSnapshot = timeSnapshot
            };
        }

        /// <summary>
        /// Stops harvesting the resource the entity is harvesting.
        /// The amount of resources collected will be calculated at this time
        /// </summary>
        public ResourceStackData StopHarvesting()
        {
            var harvesting = CurrentEntity.Components.Get<HarvestingComponent>();
            var tile = Game.World.GetTile(harvesting.Tile.X, harvesting.Tile.Y);
            var harvestState = CalculateCurrentState();
            var spec = Game.Specs.Resources[harvestState.Resources.Resource.ResourceId];

            CurrentEntity.Components.Remove<HarvestingComponent>();
            tile.Components.TryGet<TileResourceComponent>(out var tileResources);
            tileResources.BeingHarvested = false;
            tileResources.Resource.Amount = (ushort)(tileResources.Resource.Amount - harvestState.AmountHarvested);
            var finalStack = new ResourceStackData(harvestState.Resources.Resource.ResourceId, harvestState.AmountHarvested);
            var ev = EventPool<HarvestingEndedEvent>.Get();
            tile.Save(tileResources);
            ev.Tile = tile;
            ev.Harvester = CurrentEntity;
            ev.Resource = finalStack;
            CurrentEntity.Components.CallEvent(ev);
            EventPool<HarvestingEndedEvent>.Return(ev);
            Game.Log.Debug($"{CurrentEntity} finished harvesting on {tile} and got {harvestState.AmountHarvested}x{spec.Name}");
            return finalStack;
        }
    }
}