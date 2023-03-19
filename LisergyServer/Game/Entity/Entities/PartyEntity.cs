using Game.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using Game.Battles;
using Game.Events.ServerEvents;
using Game.Entity.Components;
using Game.Movement;
using Game.World.Systems;
using Game.DataTypes;
using Game.Entity.Logic;

namespace Game.Entity.Entities
{
    [Serializable]
    public class PartyEntity : WorldEntity, IBattleableEntity
    {

        [NonSerialized]
        private BattleComponentsLogic _battleLogic;

        public PartyEntity(PlayerEntity owner, byte partyIndex) : base(owner)
        {
            SetupDefaultComponents();
            PartyIndex = partyIndex;
        }

        public PartyEntity(PlayerEntity owner) : base(owner)
        {
            SetupDefaultComponents();
        }

        public IBattleComponentsLogic BattleLogic => _battleLogic;

        public bool IsAlive() => BattleGroupSystem.IsAlive(this);

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
            Components.Add(new PartyComponent());
            Components.Add(new BattleGroupComponent());
            Components.Add(new EntityVisionComponent());
            Components.Add(new EntityMovementComponent() { MoveDelay = TimeSpan.FromSeconds(0.25) });
            _battleLogic = new BattleComponentsLogic(this);
        }

        public byte PartyIndex { get => Components.Get<PartyComponent>().PartyIndex; set => Components.Get<PartyComponent>().PartyIndex = value; }

        public byte GetLineOfSight() => Components.Get<EntityVisionComponent>()?.LineOfSight ?? 0;

        public override string ToString()
        {
            return $"<Party Id={Id} Index={PartyIndex} Owner={OwnerID}>";
        }

        public ServerPacket GetStatusUpdatePacket() => new PartyStatusUpdatePacket(this);

      
    }
}
