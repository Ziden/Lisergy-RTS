using Game.Engine.DataTypes;
using Game.Engine.ECLS;
using Game.Entities;
using Game.Systems.Battle.Data;
using Game.Systems.Battler;
using Game.Systems.Building;
using Game.Systems.Party;
using Game.Tile;
using Game.World;
using GameData;
using GameData.Specs;
using System.Collections.Generic;
using System.Linq;


namespace Game.Systems.Player
{
    /// <summary>
    /// Interacts with player specific data
    /// </summary>
    public class PlayerLogic : BaseEntityLogic<PlayerDataComponent>
    {
        private PlayerDataComponent Data => CurrentEntity.Components.Get<PlayerDataComponent>();

        public Unit RecruitUnit(UnitSpecId unitSpecId)
        {
            var unit = new Unit(Game.Specs.Units[unitSpecId]);
            Data.StoredUnits.Add(unit.Id, unit);
            Game.Log.Debug($"{CurrentEntity} recruited {unit}");
            return unit;
        }

        public IReadOnlyCollection<Location> GetVisibleTiles()
        {
            CurrentEntity.Components.TryGet<PlayerVisibilityComponent>(out var visibilityData);
            return visibilityData?.VisibleTiles;
        }

        public IReadOnlyList<IEntity> GetBuildings()
        {
            return Game.Entities.GetChildren(CurrentEntity.EntityId, EntityType.Building);
        }

        public IReadOnlyList<IEntity> GetParties()
        {
            return Game.Entities.GetChildren(CurrentEntity.EntityId, EntityType.Party);
        }

        public IEntity GetCenter()
        {
            return GetBuildings().Where(b => b.Get<PlayerBuildingComponent>().SpecId == Game.Specs.InitialBuildingSpecId).First();
        }

        /// <summary>
        /// Creates a new party for the player on the given party index
        /// </summary>
        public IEntity CreateNewParty()
        {
            var entity = Game.Entities.CreateEntity(EntityType.Party, CurrentEntity.EntityId);
            var party = entity.Get<PartyComponent>();
            entity.Save(party);
            return entity;
        }

        /// <summary>
        /// Gets what the owner of the given entity can build at this moment
        /// </summary>
        public List<BuildingConstructionSpec> GetBuildingOptions()
        {
            var list = new List<BuildingConstructionSpec>();
            foreach (var kp in Game.Specs.BuildingConstructions)
            {
                list.Add(kp.Value);
            }
            return list;
        }

        /// <summary>
        /// Adds a new player to the given tile with the initial things a new player should have
        /// </summary>
        public void PlaceNewPlayer(TileModel t)
        {
            Game.Log.Debug($"Placing new player {CurrentEntity.EntityId} on tile {t}");
            if (Game.Specs.InitialBuildingSpecId.HasValue)
            {
                t.Entity.Logic.Building.ForceBuild(Game.Specs.InitialBuildingSpecId.Value, CurrentEntity.EntityId);
            }
            var initialUnit = Game.Specs.InitialUnit.SpecId;
            var unit = RecruitUnit(initialUnit);
            var party = CreateNewParty();
            PlaceUnitInParty(unit.Id, party);
            var partyTile = t.GetNeighbor(Direction.EAST);
            if (!partyTile.Logic.Tile.IsPassable()) partyTile = t.GetNeighbor(Direction.WEST);
            if (!partyTile.Logic.Tile.IsPassable()) partyTile = t.GetNeighbor(Direction.SOUTH);
            if (!partyTile.Logic.Tile.IsPassable()) partyTile = t.GetNeighbor(Direction.NORTH);
            party.Logic.Map.SetPosition(partyTile);
            Game.Log.Debug($"Placed new player {CurrentEntity} in {t}");
            return;
        }

        /// <summary>
        /// Record a battle header of a battle that happened for this player
        /// </summary>
        public void RecordBattleHeader(BattleHeader header)
        {
            Data.BattleHeaders.Add(header);
        }

        /// <summary>
        /// Moves a unit to a given party
        /// </summary>
        public void PlaceUnitInParty(GameId unitId, IEntity newParty)
        {
            if (!Data.StoredUnits.TryGetValue(unitId, out var storedUnit))
            {
                Game.Log.Error($"Tried to add unity {unitId} to party but unit was not free");
                return;
            }
            Data.StoredUnits.Remove(unitId);
            Game.Logic.GetEntityLogic(newParty).BattleGroup.AddUnit(storedUnit);
            Game.Log.Debug($"{CurrentEntity} moved unit {storedUnit} to party {newParty}");
        }


    }
}
