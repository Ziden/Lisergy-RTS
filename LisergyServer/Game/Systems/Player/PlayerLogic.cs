﻿using Game.Battle.Data;
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

        public Unit RecruitUnit(ushort unitSpecId)
        {
            var unit = new Unit(Game.Specs.Units[unitSpecId]);

            Data.Units.Add(unit);
            Log.Debug($"{Entity} recruited {unit}");
            return unit;
        }

        public PartyEntity FindParty(Unit u)
        {
            foreach (PartyEntity p in Data.Parties)
            {
                if (p.Get<BattleGroupComponent>().Units.Contains(u)) return p;
            }   
            return null;
        }

        public void PlaceNewPlayer(TileEntity t)
        {
            Game.GameWorld.Players.Add(Entity as PlayerEntity);
            var castleID = Game.Specs.InitialBuilding.Id;
            Build(castleID, t);
            ushort initialUnit = Game.Specs.InitialUnit.UnitSpecID;
            var unit = RecruitUnit(initialUnit);
            var party = Data.Parties[0];
            PlaceUnitInParty(unit, party);
            Game.Systems.Map.GetLogic(party).SetPosition(t.GetNeighbor(Direction.EAST));
            Log.Debug($"Placed new player {Entity} in {t}");
            return;
        }

        public void RecordHeader(BattleHeaderData header)
        {
            Data.BattleHeaders[header.BattleID] = header;
        }

        public void PlaceUnitInParty(Unit u, PartyEntity newParty)
        {
            var existingParty = FindParty(u);
            if (existingParty != null)
            {
                Game.Logic.GetEntityLogic(existingParty).BattleGroup.RemoveUnit(u);
            }
            Game.Logic.GetEntityLogic(newParty).BattleGroup.AddUnit(u);
            Log.Debug($"{Entity} moved unit {u} to party {newParty}");
        }

        public void Build(ushort buildingSpecId, TileEntity t)
        {
            var building = Game.Entities.CreateEntity<PlayerBuildingEntity>(Game.Players.GetPlayer(Entity.OwnerID));
            building.BuildFromSpec(Game.Specs.Buildings[buildingSpecId]);
            Data.Buildings.Add(building);
            building.EntityLogic.Map.SetPosition(t);
            Log.Debug($"Player {Entity} built {building}");
        }
    } 
}