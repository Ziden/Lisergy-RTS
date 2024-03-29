﻿using Game.Battler;
using Game.ECS;
using Game.Events;
using Game.FogOfWar;
using Game.Movement;
using Game.Player;
using System;

namespace Game.Party
{
    [Serializable]
    public class PartyEntity : WorldEntity, IBattleableEntity
    {

        public const int SIZE = 4;

        [NonSerialized]
        private BattleGroupComponentLogic _battleLogic;

        public PartyEntity(PlayerEntity owner, byte partyIndex) : base(owner)
        {
            SetupDefaultComponents();
            PartyIndex = partyIndex;
        }

        public PartyEntity(PlayerEntity owner) : base(owner)
        {
            SetupDefaultComponents();
        }

        public IBattleComponentsLogic BattleGroupLogic => _battleLogic;

        public bool CanMove()
        {
            return !_battleLogic.IsBattling;
        }

        public CourseTask Course
        {
            get => EntityMovementSystem.GetCourse(this);
            set => EntityMovementSystem.SetCourse(this, value);
        }

        private void SetupDefaultComponents()
        {
            _battleLogic = new BattleGroupComponentLogic(this);
            Components.Add(new BattleGroupComponent());
            Components.Add(new PartyComponent());
            Components.Add(new EntityVisionComponent());
            Components.Add(new EntityMovementComponent() { MoveDelay = TimeSpan.FromSeconds(0.25) });
        }

        public byte PartyIndex { get => Components.Get<PartyComponent>().PartyIndex; set => Components.Get<PartyComponent>().PartyIndex = value; }

        public byte GetLineOfSight() => Components.Get<EntityVisionComponent>()?.LineOfSight ?? 0;

        public override string ToString()
        {
            return $"<Party Id={Id} Index={PartyIndex} Owner={OwnerID}>";
        }

        public ServerPacket GetStatusUpdatePacket() => GetUpdatePacket(null);

        public IComponentEntityLogic[] GetLogicsToSync()
        {
            throw new NotImplementedException();
        }
    }
}
