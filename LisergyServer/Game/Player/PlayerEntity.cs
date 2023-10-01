using Game.Battle;
using Game.DataTypes;
using Game.ECS;
using Game.Events;
using Game.Systems.Battler;
using Game.Systems.Building;
using Game.Systems.FogOfWar;
using Game.Systems.Party;
using Game.Tile;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.Player
{
    public abstract class PlayerEntity : IEntity
    {
        public GameId UserID;

        public HashSet<Unit> Units = new HashSet<Unit>();
        public HashSet<PlayerBuildingEntity> Buildings = new HashSet<PlayerBuildingEntity>();
        public HashSet<TileEntity> VisibleTiles = new HashSet<TileEntity>();
        public HashSet<TileEntity> OnceExplored = new HashSet<TileEntity>();

        public PartyEntity[] Parties;

        public Dictionary<GameId, CompleteBattleHeader> BattleHeaders = new Dictionary<GameId, CompleteBattleHeader>();

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
            Parties = new PartyEntity[PartyEntity.SIZE]
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
            unit.SetBaseStats();
            Units.Add(unit);
            Log.Debug($"{UserID} recruited {unitSpecId}");
            return unit;
        }

        public void PlaceUnitInParty(Unit u, PartyEntity newParty)
        {
            if (u.Party != null)
                u.Party.BattleGroupLogic.RemoveUnit(u);
            u.Party = newParty;
            newParty.BattleGroupLogic.AddUnit(u);
            Log.Debug($"{UserID} moved unit {u.SpecId} to party {newParty.PartyIndex}");
        }

        public void Build(ushort id, TileEntity t)
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
