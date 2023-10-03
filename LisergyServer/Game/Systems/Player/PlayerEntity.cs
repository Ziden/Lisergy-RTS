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

namespace Game.Systems.Player
{
    public abstract class PlayerEntity 
    {
        public GameId UserID;

        public HashSet<Unit> Units = new HashSet<Unit>();
        public HashSet<PlayerBuildingEntity> Buildings = new HashSet<PlayerBuildingEntity>();
        public HashSet<TileEntity> VisibleTiles = new HashSet<TileEntity>();
        public HashSet<TileEntity> OnceExplored = new HashSet<TileEntity>();

        public PartyEntity[] Parties;

        public Dictionary<GameId, CompleteBattleHeader> BattleHeaders = new Dictionary<GameId, CompleteBattleHeader>();

        public IComponentSet Components => throw new NotImplementedException();

        public GameLogic Game { get; private set; }

        public PartyEntity GetParty(byte partyIndex)
        {
            return Parties[partyIndex];
        }

        public bool CanReceivePackets()
        {
            return Online() && UserID != GameId.ZERO;
        }

        public PlayerEntity(GameLogic game)
        {
            Game = game;
            UserID = Guid.NewGuid();
            Parties = new PartyEntity[PartyEntity.SIZE]
            {
                new PartyEntity(this, 0), null, null, null
            };
        }

        public PlayerBuildingEntity GetCenter()
        {
            return Buildings.First(b => b.SpecID == GameLogic.Specs.InitialBuilding);
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
            {
                Game.Systems.BattleGroup.GetLogic(u.Party).RemoveUnit(u);
            }
            u.Party = newParty;
            Game.Systems.BattleGroup.GetLogic(u.Party).AddUnit(u);
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

        public PlayerEntity Owner => this;

        public GameId OwnerID => UserID;

        public abstract bool Online();

        public override string ToString()
        {
            return $"<Player id={UserID.ToString()}>";
        }

        public ServerPacket GetUpdatePacket(PlayerEntity receiver)
        {
            throw new NotImplementedException();
        }

        public T Get<T>() where T : IComponent
        {
            return default;
        }
    }
}
