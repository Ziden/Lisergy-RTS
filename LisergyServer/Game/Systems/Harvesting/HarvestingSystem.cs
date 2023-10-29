using Game.ECS;
using Game.Systems.Course;
using Game.Systems.Map;
using Game.Systems.Movement;

namespace Game.Systems.Resources
{
    public unsafe class HarvestingSystem : LogicSystem<HarvesterComponent, HarvestingLogic>
    {
        public HarvestingSystem(LisergyGame game) : base(game) {}

        public override void OnEnabled()
        {
            EntityEvents.On<CourseFinishEvent>(OnCourseFinish);
            EntityEvents.On<EntityMoveOutEvent>(OnMoveOutTile);
        }

        private void OnMoveOutTile(IEntity e, EntityMoveOutEvent ev)
        {
            var logic = GetLogic(e);
            if (logic.IsHarvesting()) logic.StopHarvesting();
        }

        private void OnCourseFinish(IEntity entity, CourseFinishEvent ev)
        {
            if (ev.Intent != CourseIntent.Harvest) return;
            var logic = GetLogic(entity);
            if (logic.CanHarvest(ev.ToTile)) logic.StartHarvesting(ev.ToTile);
        }

    }
}
