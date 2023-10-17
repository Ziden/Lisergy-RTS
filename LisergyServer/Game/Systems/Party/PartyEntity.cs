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

        public PartyEntity(IGame game, GameId owner) : base(game, owner)
        {
            Components.Add<MapPlacementComponent>();
            Components.Add<BattleGroupComponent>();
            Components.Add<PartyComponent>();
            Components.Add<EntityVisionComponent>();
            Components.Add<CourseComponent>();
            Components.Add<MovespeedComponent>();
            Components.Get<MovespeedComponent>().MoveDelay = TimeSpan.FromSeconds(0.25);
            Components.AddReference(new MapReferenceComponent());
        }

        public bool CanMove()
        {
            return Get<BattleGroupComponent>().BattleID == GameId.ZERO;
        }

        // TODO: Remove this
        public CourseTask Course
        {
            get => (CourseTask)EntityLogic.Movement.GetCourse();
            set => EntityLogic.Movement.SetCourse(value);
        }

        public ref readonly byte PartyIndex { get => ref Components.Get<PartyComponent>().PartyIndex; }
        public ref readonly byte GetLineOfSight() => ref Components.Get<EntityVisionComponent>().LineOfSight;
        public override string ToString()
        {
            return $"<Party Entity={EntityId} Index={PartyIndex} Owner={OwnerID}>";
        }
    }
}
