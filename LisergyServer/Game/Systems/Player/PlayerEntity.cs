﻿using Game.DataTypes;
using Game.ECS;
using Game.Events;
using Game.Events.ServerEvents;
using Game.Network;
using Game.Systems.Building;
using Game.Systems.Party;
using System;
using System.Linq;

namespace Game.Systems.Player
{
    public class PlayerEntity : IEntity
    {
        public const byte MAX_PARTIES = 4;

        protected GameId _playerId;

        public PlayerDataComponent Data => Components.GetReference<PlayerDataComponent>();

        public ref readonly GameId EntityId => ref _playerId;
        public ref readonly GameId OwnerID => ref _playerId;

        public IGame Game { get; private set; }
        public PlayerEntity(IGame game)
        {
            Game = game;
            Components = new ComponentSet(this, this);
            Components.Add<PlayerComponent>();
            Components.AddReference(new PlayerDataComponent());
            _playerId = GameId.Generate();
            Data.Parties = new PartyEntity[MAX_PARTIES]
            {
                game.Entities.CreateEntity<PartyEntity>(this),  
                game.Entities.CreateEntity<PartyEntity>(this), 
                game.Entities.CreateEntity<PartyEntity>(this),  
                game.Entities.CreateEntity<PartyEntity>(this)
            };
            for (byte x = 0; x < MAX_PARTIES; x++)
            {
                var entity = Data.Parties[x];
                var party = entity.Get<PartyComponent>();
                party.PartyIndex = x;
                entity.Save(party);
            }
            
        }

        public PartyEntity GetParty(byte partyIndex) => Data.Parties[partyIndex];

        public PlayerBuildingEntity GetCenter()
        {
            return Data.Buildings.First(b => b.SpecId == Game.Specs.InitialBuilding.Id);
        }

        public void SendMessage(string msg, MessageType type = MessageType.RAW_TEXT)
        {
            Game.Network.SendToPlayer(new MessagePacket(MessageType.BAD_INPUT, msg), this);
        }

        public IComponentSet Components { get; private set; }
        public IEntityLogic EntityLogic => Game.Logic.GetEntityLogic(this);
        public ref DeltaFlags DeltaFlags => throw new NotImplementedException();
        public EntityType EntityType => EntityType.Player;
        public override string ToString() => $"<Player id={EntityId}>";
        public ref T Get<T>() where T : unmanaged, IComponent => ref Components.Get<T>();
        public BasePacket GetUpdatePacket(PlayerEntity receiver) => throw new NotImplementedException();
        public void ProccessDeltas(PlayerEntity trigger) => throw new NotImplementedException();
    }
}