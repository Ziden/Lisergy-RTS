using Game.DataTypes;
using Game.ECS;
using Game.Entity;
using Game.Entity.Components;
using Game.Events;
using Game.World;
using Game.World.Components;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.Entity.Entities
{
    public abstract class PlayerEntity : IEntity
    {
        public GameId UserID;

        public HashSet<Unit> Units = new HashSet<Unit>();
        public HashSet<PlayerBuildingEntity> Buildings = new HashSet<PlayerBuildingEntity>();
        public HashSet<Tile> VisibleTiles = new HashSet<Tile>();
        public HashSet<Tile> OnceExplored = new HashSet<Tile>();

        public PartyEntity[] Parties; // refactor to stop using in client to set clientparty instances

        public List<BattleResultPacket> Battles = new List<BattleResultPacket>();

        public IComponentSet Components => throw new NotImplementedException();

        public PartyEntity GetParty(byte partyIndex)
        {
            return Parties[partyIndex];
        }

        public bool CanReceivePackets()
        {
            return Online() && !Gaia.IsGaia(UserID);
        }

        public PlayerEntity()
        {
            UserID = Guid.NewGuid();
            Parties = new PartyEntity[4]
            {
                new PartyEntity(this, 0),new PartyEntity(this, 1),new PartyEntity(this, 2),new PartyEntity(this, 3),
            };
        }

        public PlayerBuildingEntity GetCenter()
        {
            return Buildings.First(b => b.SpecID == StrategyGame.Specs.InitialBuilding);
        }

        public Unit RecruitUnit(ushort unitSpecId)
        {
            var unit = new Unit(unitSpecId);
            unit.SetSpecStats();
            Units.Add(unit);
            Log.Debug($"{UserID} recruited {unitSpecId}");
            return unit;
        }

        public void PlaceUnitInParty(Unit u, PartyEntity newParty)
        {
            if (u.Party != null)
                u.Party.BattleLogic.RemoveUnit(u);
            u.Party = newParty;
            newParty.BattleLogic.AddUnit(u);
            Log.Debug($"{UserID} moved unit {u.SpecId} to party {newParty.PartyIndex}");
        }

        public void Build(ushort id, Tile t)
        {
            var building = new PlayerBuildingEntity(this);
            building.Components.Add(new PlayerBuildingComponent() { SpecId = id });
            var los = building.GetSpec().LOS;
            building.Components.Add(new EntityVisionComponent() { LineOfSight = los });
            Buildings.Add(building);
            building.Tile = t;
            Log.Debug($"Player {UserID} built {id}");
        }

        public abstract void Send<EventType>(EventType ev) where EventType : BaseEvent;

        public GameId EntityId => UserID;

        public abstract bool Online();

        public override string ToString()
        {
            return $"<Player id={UserID.ToString()}>";
        }
    }
}
