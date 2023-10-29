using Game.DataTypes;
using Game.Systems.Battler;
using Game.Systems.FogOfWar;
using Game.Systems.Map;
using Game.Systems.MapPosition;
using Game.Systems.Movement;
using System;
using Game.Scheduler;
using Game.Systems.Resources;

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
            Components.Add<CargoComponent>();
            Components.Add<HarvesterComponent>();
            Components.Get<CargoComponent>().MaxWeight = game.Specs.Harvesting.StartingPartyCargoWeight;
            Components.Get<MovespeedComponent>().MoveDelay = TimeSpan.FromSeconds(0.3);
            Components.AddReference(new MapReferenceComponent());
        }
        
        public GameTask Course => EntityLogic.Movement.GetCourseTask();
        public ref readonly byte PartyIndex { get => ref Components.Get<PartyComponent>().PartyIndex; }
        public ref readonly byte GetLineOfSight() => ref Components.Get<EntityVisionComponent>().LineOfSight;
        public override string ToString() => $"<Party Entity={EntityId} Index={PartyIndex} Owner={OwnerID}>";
    }
}
