using Game.DataTypes;
using Game.ECS;
using Game.Events;
using Game.Network;
using Game.Systems.Building;
using Game.Systems.FogOfWar;
using Game.Systems.Party;
using Game.Tile;
using System;
using System.Linq;

namespace Game.Systems.Player
{
    public abstract class PlayerEntity : IEntity
    {
        public PlayerDataComponent Data => Get<PlayerDataComponent>();

        public const byte MAX_PARTIES = 4;
        public GameId EntityId { get; }
        public IGame Game { get; private set; }
        public bool CanReceivePackets() => Online() && EntityId != GameId.ZERO;

        public PlayerEntity(IGame game)
        {
            Game = game;
            Components = new ComponentSet(this, this);
            Components.Add<PlayerDataComponent>();
            EntityId = Guid.NewGuid();
            Data.Parties = new PartyEntity[MAX_PARTIES]
            {
                game.Entities.CreateEntity<PartyEntity>(this),  
                game.Entities.CreateEntity<PartyEntity>(this), 
                game.Entities.CreateEntity<PartyEntity>(this),  
                game.Entities.CreateEntity<PartyEntity>(this)
            };
            for (byte x = 0; x < MAX_PARTIES; x++) Data.Parties[x].Get<PartyComponent>().PartyIndex = x;
        }

        public PartyEntity GetParty(byte partyIndex) => Data.Parties[partyIndex];

        public PlayerBuildingEntity GetCenter()
        {
            return Data.Buildings.First(b => b.SpecId == Game.Specs.InitialBuilding.Id);
        }

        public abstract void Send<EventType>(EventType ev) where EventType : BaseEvent;

        public PlayerEntity Owner => this;

        public GameId OwnerID => EntityId;

        public IComponentSet Components { get; private set; }

        public IEntityLogic EntityLogic => Game.Logic.EntityLogic(this);

        public ref DeltaFlags DeltaFlags => throw new NotImplementedException();

        public abstract bool Online();

        public override string ToString()
        {
            return $"<Player id={EntityId.ToString()}>";
        }

        public ServerPacket GetUpdatePacket(PlayerEntity receiver)
        {
            throw new NotImplementedException();
        }

        public T Get<T>() where T : IComponent => Components.Get<T>();

        public void ProccessDeltas(PlayerEntity trigger)
        {
            throw new NotImplementedException();
        }
    }
}
