using Game.ECS;
using Game.Events.GameEvents;
using Game.Systems.Battler;
using Game.Systems.Tile;

namespace Game.Systems.Movement
{
    public class CourseSystem : LogicSystem<CourseComponent, CourseLogic>
    {
        public CourseSystem(LisergyGame game) : base(game) { }

        public override void OnEnabled()
        {
            EntityEvents.On<EntityMoveInEvent>(OnEntityMoveIn);
        }

        private void OnEntityMoveIn(IEntity owner, EntityMoveInEvent ev)
        {
            var tileHabitants = ev.ToTile.Components.GetReference<TileHabitants>();
            var course = ev.Entity.EntityLogic.Movement.TryGetCourseTask();

            if (course == null || course.Intent != CourseIntent.OffensiveTarget || !course.IsLastMovement()) return;
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
