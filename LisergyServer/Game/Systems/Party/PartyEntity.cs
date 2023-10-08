using Game.DataTypes;
using Game.Systems.Battler;
using Game.Systems.FogOfWar;
using Game.Systems.Map;
using Game.Systems.MapPosition;
using Game.Systems.Movement;
using Game.Systems.Player;
using System;
using System.Collections.Generic;

namespace Game.Systems.Party
{
    public class PartyEntity : BaseEntity
    {
        public const int SIZE = 4;

        public override EntityType EntityType => EntityType.Party;

        public PartyEntity(IGame game, PlayerEntity owner) : base(game, owner)
        {
            Components.Add(new MapPlacementComponent());
            Components.Add(new MapReferenceComponent());
            Components.Add(new BattleGroupComponent());
            Components.Add(new PartyComponent());
            Components.Add(new EntityVisionComponent());
            Components.Add(new EntityMovementComponent() { MoveDelay = TimeSpan.FromSeconds(0.25) });
        }

        public bool CanMove()
        {
            return Get<BattleGroupComponent>().BattleID == GameId.ZERO;
        }

        public CourseTask Course
        {
            get => EntityLogic.Movement.GetCourse();
            set => EntityLogic.Movement.SetCourse(value);
        }

        public byte PartyIndex { get => Components.Get<PartyComponent>().PartyIndex; }

        public byte GetLineOfSight() => Components.Get<EntityVisionComponent>().LineOfSight;

        public override string ToString()
        {
            return $"<Party Entity={EntityId} Index={PartyIndex} Owner={OwnerID}>";
        }
    }
}
