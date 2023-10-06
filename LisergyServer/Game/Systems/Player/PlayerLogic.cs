using Game.ECS;
using Game.Systems.Battler;
using Game.Systems.Building;
using Game.Systems.Party;
using Game.Systems.Tile;
using Game.Tile;
using Game.World;

namespace Game.Systems.Player
{
    public class PlayerLogic : BaseComponentLogic<PlayerDataComponent>
    {
        public Unit RecruitUnit(ushort unitSpecId)
        {
            var unit = new Unit(Game.Specs.Units[unitSpecId]);
            Component.Units.Add(unit);
            Log.Debug($"{Entity.EntityId} recruited {unitSpecId}");
            return unit;
        }

        public PartyEntity FindParty(Unit u)
        {
            foreach (PartyEntity p in Component.Parties) if (p.Get<BattleGroupComponent>().Units.Contains(u)) return p;
            return null;
        }

        public void PlaceNewPlayer(TileEntity t)
        {
            Game.GameWorld.Players.Add(Entity as PlayerEntity);
            var castleID = Game.Specs.InitialBuilding.Id;
            Build(castleID, t);
            ushort initialUnit = Game.Specs.InitialUnit.UnitSpecID;
            var unit = RecruitUnit(initialUnit);
            var party = Component.Parties[0];
            PlaceUnitInParty(unit, party);
            Game.Systems.Map.GetEntityLogic(party).SetPosition(t.GetNeighbor(Direction.EAST));
            Log.Debug($"Placed new player in {t}");
            return;
        }

        public void PlaceUnitInParty(Unit u, PartyEntity newParty)
        {
            var existingParty = FindParty(u);
            if (existingParty != null)
            {
                Game.Logic.EntityLogic(existingParty).BattleGroup.RemoveUnit(u);
            }
            Game.Logic.EntityLogic(newParty).BattleGroup.AddUnit(u);
            Log.Debug($"{Entity.OwnerID} moved unit {u.SpecId} to party {newParty.PartyIndex}");
        }

        public void Build(ushort buildingSpecId, TileEntity t)
        {
            var building = Game.Entities.CreateEntity<PlayerBuildingEntity>(Game.Players.GetPlayer(Entity.OwnerID));
            building.BuildFromSpec(Game.Specs.Buildings[buildingSpecId]);
            Component.Buildings.Add(building);
            Game.Logic.Map(building).SetPosition(t);
            Log.Debug($"Player {Entity.OwnerID} built {buildingSpecId}");
        }
    } 
}
