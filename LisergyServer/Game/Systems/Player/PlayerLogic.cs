using Game.Battle.Data;
using Game.DataTypes;
using Game.ECS;
using Game.Network.ServerPackets;
using Game.Systems.Battler;
using Game.Systems.Building;
using Game.Systems.Party;
using Game.Systems.Tile;
using Game.Tile;
using Game.World;

namespace Game.Systems.Player
{
    public class PlayerLogic : BaseEntityLogic<PlayerComponent>
    {
        private PlayerDataComponent Data => Entity.Components.GetReference<PlayerDataComponent>();

        /// <summary>
        /// Recruits a new unit for the player
        /// </summary>
        public Unit RecruitUnit(ushort unitSpecId)
        {
            var unit = new Unit(Game.Specs.Units[unitSpecId]);

            Data.Units.Add(unit);
            Game.Log.Debug($"{Entity} recruited {unit}");
            return unit;
        }

        /// <summary>
        /// Finds the party of a given unit
        /// </summary>
        public PartyEntity FindParty(Unit u)
        {
            foreach (PartyEntity p in Data.Parties)
            {
                if (p.Get<BattleGroupComponent>().Units.Contains(u)) return p;
            }   
            return null;
        }

        /// <summary>
        /// Adds a new player to the given tile with the initial things a new player should have
        /// </summary>
        public void PlaceNewPlayer(TileEntity t)
        {
            var p = Entity as PlayerEntity;
            Game.Players.Add(p);
            var castleID = Game.Specs.InitialBuilding.Id;
            Build(castleID, t);
            ushort initialUnit = Game.Specs.InitialUnit.UnitSpecID;
            var unit = RecruitUnit(initialUnit);
            var party = Data.Parties[0];
            PlaceUnitInParty(unit, party);
            var partyTile = t.GetNeighbor(Direction.EAST);
            if(!partyTile.Passable) partyTile = t.GetNeighbor(Direction.WEST);
            if (!partyTile.Passable) partyTile = t.GetNeighbor(Direction.SOUTH);
            if (!partyTile.Passable) partyTile = t.GetNeighbor(Direction.NORTH);
            Game.Systems.Map.GetLogic(party).SetPosition(partyTile);
            Game.Log.Debug($"Placed new player {Entity} in {t}");
            return;
        }

        /// <summary>
        /// Record a battle header of a battle that happened for this player
        /// </summary>
        public void RecordBattleHeader(BattleHeaderData header)
        {
            Data.BattleHeaders[header.BattleID] = header;
        }

        /// <summary>
        /// Moves a unit to a given party
        /// </summary>
        public void PlaceUnitInParty(Unit u, PartyEntity newParty)
        {
            var existingParty = FindParty(u);
            if (existingParty != null)
            {
                Game.Logic.GetEntityLogic(existingParty).BattleGroup.RemoveUnit(u);
            }
            Game.Logic.GetEntityLogic(newParty).BattleGroup.AddUnit(u);
            Game.Log.Debug($"{Entity} moved unit {u} to party {newParty}");
        }

        /// <summary>
        /// Builds a new building on the given tile
        /// </summary>
        public PlayerBuildingEntity Build(ushort buildingSpecId, TileEntity t)
        {
            var building = (PlayerBuildingEntity)Game.Entities.CreateEntity(Entity.OwnerID, EntityType.Building);
            building.BuildFromSpec(Game.Specs.Buildings[buildingSpecId]);
            Data.Buildings.Add(building);
            building.EntityLogic.Map.SetPosition(t);
            Game.Log.Debug($"Player {Entity} built {building}");
            return building;
        }
    } 
}
