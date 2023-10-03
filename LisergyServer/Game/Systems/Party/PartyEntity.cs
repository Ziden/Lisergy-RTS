using Game.DataTypes;
using Game.ECS;
using Game.Events;
using Game.Systems.Battler;
using Game.Systems.FogOfWar;
using Game.Systems.Movement;
using Game.Systems.Player;
using System;

namespace Game.Systems.Party
{
    [Serializable]
    public class PartyEntity : BaseEntity
    {
        public const int SIZE = 4;

        public PartyEntity(PlayerEntity owner, byte partyIndex = 0) : base(owner)
        {
            Components.Add(new BattleGroupComponent());
            Components.Add(new PartyComponent()
            { 
                PartyIndex = partyIndex
            });
            Components.Add(new EntityVisionComponent());
            Components.Add(new EntityMovementComponent() { MoveDelay = TimeSpan.FromSeconds(0.25) });
        }

        //public BattleGroupComponentLogic BattleGroupLogic => _battleLogic;

        public bool CanMove()
        {
            return Get<BattleGroupComponent>().BattleID == GameId.ZERO;
        }

        public CourseTask Course
        {
            get => EntityMovementSystem.GetCourse(this);
            set => EntityMovementSystem.SetCourse(this, value);
        }

        public byte PartyIndex { get => Components.Get<PartyComponent>().PartyIndex; }

        public byte GetLineOfSight() => Components.Get<EntityVisionComponent>()?.LineOfSight ?? 0;

        public override string ToString()
        {
            return $"<Party Id={Id} Index={PartyIndex} Owner={OwnerID}>";
        }
    }
}
