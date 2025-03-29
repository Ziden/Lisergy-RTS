using Game.Engine.DataTypes;
using Game.Engine.ECLS;
using Game.Entities;
using Game.Systems.FogOfWar;
using Game.Tile;
using Game.World;
using GameData;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.Systems.Building
{
    public enum BuildResult { Ok, Blocked, HasResource, HasBuilding }

    public enum BuildingTechStatus
    {
        Available,
        NotResearched,
        NotInTechTree
    }

    public class BuildingTechResult
    {
        public BuildingTechStatus Status { get; set; }
        public BuildingSpecId? BlockedBy { get; set; }
        public bool IsAvailable => Status == BuildingTechStatus.Available;
    }

    public unsafe class ConstructionLogic : BaseEntityLogic<ConstructionWorkerComponent>
    {
        public BuildingSpec SetupBuildingFromSpec(IEntity building, BuildingSpecId specId)
        {
            var spec = Game.Specs.Buildings[specId];
            building.Components.Add(new PlayerBuildingComponent()
            {
                SpecId = specId
            });
            building.Components.Add(new EntityVisionComponent()
            {
                LineOfSight = spec.LOS
            });
            return spec;
        }

        public IEntity ForceBuild(BuildingSpecId buildingSpecId, GameId owner)
        {
            var t = CurrentEntity.Logic.Map.GetTile();
            t.Logic.Tile.SetTileId(0);
            var building = PlaceConstruction(buildingSpecId, owner);
            building.Components.Remove<ConstructionSiteComponent>();
            Game.Log.Debug($"Player {CurrentEntity} insta-built {building}");

            return building;
        }

        public IEntity PlaceConstruction(BuildingSpecId buildingSpecId, GameId owner)
        {
            var t = CurrentEntity.Logic.Map.GetTile();
            var canBuild = IsTileFreeForBuilding(buildingSpecId);
            if (canBuild != BuildResult.Ok)
            {
                Game.Log.Error($"Cannot build building spec {buildingSpecId} at {t}: {canBuild}");
                return null;
            }
            var building = Game.Entities.CreateEntity(EntityType.Building, owner);
            var spec = SetupBuildingFromSpec(building, buildingSpecId);
            if (Game.Specs.BuildingConstructions.TryGetValue(buildingSpecId, out var constructionSpec) && constructionSpec.TimeToBuildSeconds > 0)
            {
                var c = new ConstructionSiteComponent();
                building.Components.Add(c);
                building.Save(c);
            }
            building.Logic.Map.SetPosition(t);
            Game.Log.Debug($"Player {CurrentEntity} built {building}");
            return building;
        }

        public bool IsConstruction()
        {
            return CurrentEntity.Components.Has<ConstructionSiteComponent>();
        }

        public BuildResult IsTileFreeForBuilding(BuildingSpecId id)
        {
            if (!CurrentEntity.Logic.Tile.IsPassable()) return BuildResult.Blocked; ;
            if (CurrentEntity.Logic.Tile.GetBuildingOnTile() != null) return BuildResult.HasBuilding;
            if (CurrentEntity.Logic.Harvesting.HasHarvestingResources()) return BuildResult.HasResource;
            return BuildResult.Ok;
        }

        public bool AddBuilder(IEntity builder)
        {
            var building = CurrentEntity;
            var buildingTile = building.Logic.Map.GetTile();
            var builderTile = builder.Logic.Map.GetTile();
            if (buildingTile.Distance(builderTile) > 1)
            {
                Game.Log.Error($"{builder} is too far from {building}");
                return false;
            }
            var c = building.Components.Get<ConstructionSiteComponent>();
            c.EntitiesBuilding = c.EntitiesBuilding ?? new List<GameId>();
            c.EntitiesBuilding.Add(builder.EntityId);
            builder.Components.Add(new ConstructionWorkerComponent()
            {
                BuildingAt = buildingTile.Position,
            });
            UpdateBuildingConstructionState(c);
            Game.Log.Debug($"{builder} started to build {c}");
            building.Save(c);
            return true;
        }

        public void RemoveBuilder(IEntity builder)
        {
            var c = CurrentEntity.Components.Get<ConstructionSiteComponent>();
            c.EntitiesBuilding.Remove(builder.EntityId);
            builder.Components.Remove<ConstructionWorkerComponent>();
            UpdateBuildingConstructionState(c);
            if (c.Percentage >= 100)
            {
                CurrentEntity.Components.Remove<ConstructionSiteComponent>();
                Game.Log.Debug($"{builder} finished building {c}");
            }
            else
            {
                Game.Log.Debug($"{builder} stopped building building {c}");
                CurrentEntity.Save(c);
            }
        }

        public void UpdateBuildingConstructionState(ConstructionSiteComponent c = null)
        {
            c = c ?? CurrentEntity.Components.Get<ConstructionSiteComponent>();
            if (c.BuildingWorkPrediction != null)
            {
                var snapShot = c.BuildingWorkPrediction.GetCurrentSnapshot(Game.Scheduler.LogicalTime);
                if (snapShot.Percentagage > 1)
                {
                    c.Percentage = 100;
                }
                else c.Percentage = (byte)(snapShot.Percentagage * 100);
            }
            var b = CurrentEntity.Components.Get<PlayerBuildingComponent>();
            if (c.EntitiesBuilding.Count > 0)
            {
                var constructionSpec = Game.Specs.BuildingConstructions[b.SpecId];
                var startTime = Game.Scheduler.LogicalTime;
                var endTime = startTime + TimeSpan.FromSeconds(constructionSpec.TimeToBuildSeconds);
                c.BuildingWorkPrediction = new TimeBlock()
                {
                    StartTime = startTime,
                    EndTime = endTime
                };
            }
            else
            {
                c.BuildingWorkPrediction = null; // no one building
            }
        }
    }
}


