using Game.DataTypes;
using Game.Entity;
using Game.Entity.Components;
using Game.Events;
using Game.World;
using Game.World.Components;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Game
{
    public abstract class PlayerEntity
    {
        public GameId UserID;

        public HashSet<Unit> Units = new HashSet<Unit>();
        public HashSet<BuildingEntity> Buildings = new HashSet<BuildingEntity>();
        public HashSet<Tile> VisibleTiles = new HashSet<Tile>();
        public HashSet<Tile> OnceExplored = new HashSet<Tile>();

        public Party[] Parties; // refactor to stop using in client to set clientparty instances

        public List<BattleResultPacket> Battles = new List<BattleResultPacket>();

        public Party GetParty(byte partyIndex)
        {
            return Parties[partyIndex];
        }

        public bool CanReceivePackets()
        {
            return Online() && !Gaia.IsGaia(UserID);
        }

        public PlayerEntity()
        {
            this.UserID = Guid.NewGuid();
            Parties = new Party[4]
            {
                new Party(this, 0),new Party(this, 1),new Party(this, 2),new Party(this, 3),
            };
        }

        public BuildingEntity GetCenter()
        {
            return Buildings.First(b => b.SpecID == StrategyGame.Specs.InitialBuilding);
        }

        public Unit RecruitUnit(ushort unitSpecId)
        {
            var unit = new Unit(unitSpecId);
            unit.SetSpecStats();
            this.Units.Add(unit);
            Log.Debug($"{UserID} recruited {unitSpecId}");
            return unit;
        }

        public void PlaceUnitInParty(Unit u, Party newParty)
        {
            if (u.Party != null)
                u.Party.RemoveUnit(u);
            u.Party = newParty;
            newParty.AddUnit(u);
            Log.Debug($"{UserID} moved unit {u.SpecId} to party {newParty.PartyIndex}");
        }

        public void Build(ushort id, Tile t)
        {
            var building = new BuildingEntity(this);
            building.Components.Add(new PlayerBuildingComponent() {  SpecId = id });
            building.Components.Add(new EntityExplorationComponent() { LineOfSight = building.GetSpec().LOS });
            this.Buildings.Add(building);
            building.Tile = t;
            Log.Debug($"Player {UserID} built {id}");
        }

        public abstract void Send<EventType>(EventType ev) where EventType : BaseEvent;

        public abstract bool Online();

        public override string ToString()
        {
            return $"<Player id={UserID.ToString()}>";
        }
    }
}
