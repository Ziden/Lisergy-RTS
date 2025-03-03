using Game.Engine.ECLS;
using Game.Engine.Events;
using Game.Systems.BattleGroup;
using Game.Systems.Course;
using Game.Systems.Movement;
using Game.Systems.Tile;

namespace Game.Systems.Battler
{
    public unsafe class BattleGroupSystem : LogicSystem<BattleGroupComponent, BattleGroupLogic>
    {
        public BattleGroupSystem(LisergyGame game) : base(game) { }
        public override void RegisterListeners()
        {
            EntityEvents.On<BattleFinishedEvent>(OnBattleFinish);
            EntityEvents.On<CourseFinishEvent>(OnCourseFinish);
        }


        private void OnCourseFinish(IEntity entity, CourseFinishEvent ev)
        {
            var tileHabitants = ev.ToTile.Components.Get<TileHabitantsComponent>();

            if (ev.Intent != CourseIntent.OffensiveTarget) return;
            if (tileHabitants.Building == null) return;

            if (!ev.Entity.Components.Has<BattleGroupComponent>() || !tileHabitants.Building.Components.Has<BattleGroupComponent>()) return;
            var atkGroup = ev.Entity.Components.Get<BattleGroupComponent>();
            var defGroup = tileHabitants.Building.Components.Get<BattleGroupComponent>();
            if (GetLogic(ev.Entity).IsBattling || GetLogic(tileHabitants.Building).IsBattling) return;
            var battleId = GetLogic(ev.Entity).StartBattle(tileHabitants.Building);
        }



        /// <summary>
        /// When a battle finished processing in game logic
        /// </summary>
        private void OnBattleFinish(IEntity e, BattleFinishedEvent ev)
        {
            var logic = GetLogic(e);
            logic.ClearBattleId();
            if (logic.IsDestroyed)
            {
                var groupDead = EventPool<GroupDeadEvent>.Get();
                groupDead.Entity = e;
                e.Components.CallEvent(groupDead);
                EventPool<GroupDeadEvent>.Return(groupDead);
            }
        }
    }
}
