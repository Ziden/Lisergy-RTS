using Game.DataTypes;
using Game.ECS;
using Game.Events.GameEvents;
using Game.Systems.Battler;
using Game.Systems.Movement;

namespace Game.Systems.Tile
{
    public class TileHabitantsSystem : GameSystem<TileComponent>
    {
        public TileHabitantsSystem(LisergyGame game) : base(game) { }
        public override void OnEnabled()
        {
            EntityEvents.On<BuildingPlacedEvent>(OnStaticEntityPlaced);
            EntityEvents.On<BuildingRemovedEvent>(OnStaticEntityRemoved);
            EntityEvents.On<EntityMoveOutEvent>(OnEntityMoveOut);
            EntityEvents.On<EntityMoveInEvent>(OnEntityMoveIn);
        }

        private void OnStaticEntityRemoved(IEntity owner, BuildingRemovedEvent entity)
        {
            owner.Components.GetReference<TileHabitants>().Building = null;
        }

        private void OnStaticEntityPlaced(IEntity owner, BuildingPlacedEvent ev)
        {
            owner.Components.GetReference<TileHabitants>().Building = ev.Entity;
        }

        private void OnEntityMoveOut(IEntity owner, EntityMoveOutEvent ev)
        {
            owner.Components.GetReference<TileHabitants>().EntitiesIn.Remove(ev.Entity);
        }

        private void OnEntityMoveIn(IEntity owner, EntityMoveInEvent ev)
        {
            if (!ev.Entity.Components.Has<EntityMovementComponent>()) return;

            var tileHabitants = owner.Components.GetReference<TileHabitants>();
            // TODO: Move all this logic to battle logic
            tileHabitants.EntitiesIn.Add(ev.Entity);
            var course = ev.Entity.EntityLogic.Movement.TryGetCourseTask();
     
            if (course == null || course.Intent != MovementIntent.Offensive || !course.IsLastMovement()) return;
            if (tileHabitants.Building == null) return;

            if (!ev.Entity.Components.Has<BattleGroupComponent>() || !tileHabitants.Building.Components.Has<BattleGroupComponent>()) return;
            var atkGroup = ev.Entity.Components.Get<BattleGroupComponent>();
            var defGroup = tileHabitants.Building.Components.Get<BattleGroupComponent>();
            ev.Entity.Components.CallEvent(new OffensiveActionEvent()
            {
                AttackerGroup = atkGroup,
                DefenderGroup = defGroup,
                Defender = tileHabitants.Building,
                Attacker = ev.Entity
            });
        }
    }
}
