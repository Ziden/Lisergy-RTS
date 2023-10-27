using System;
using Game.DataTypes;
using Game.ECS;
using Game.Scheduler;
using Game.Systems.Map;
using Game.World;
using System.Collections.Generic;

namespace Game.Systems.Movement
{
    public unsafe class CourseLogic : BaseEntityLogic<CourseComponent>
    {
        public bool TryStartMovement(List<Position> sentPath, CourseIntent intent)
        {
            var owner = Game.Players.GetPlayer(Entity.OwnerID);
            foreach (var position in sentPath)
            {
                var tile = Game.World.Map.GetTile(position.X, position.Y);
                if (!tile.Passable)
                {
                    Game.Log.Error($"Impassable TileEntity {tile} in course path: {owner} moving {Entity}");
                    return false;
                }
            }
            var movement = Entity.Components.GetPointer<CourseComponent>();
            var taskExecutor = new CourseTaskExecutor(Entity, sentPath, intent);
            var task = new GameTask(Game, TimeSpan.FromMilliseconds(1), owner, taskExecutor);
            Game.Scheduler.Add(task);
            movement->CourseId = task.ID;
            movement->MovementIntent = intent;
            return true;
        }

        public void FinishCourse()
        {
            var movement = Entity.Components.GetPointer<CourseComponent>();
            movement->CourseId = GameId.ZERO;
        }

        public GameTask GetCourseTask() => Game.Scheduler.GetTask(Entity.Components.Get<CourseComponent>().CourseId);

        public GameTask? TryGetCourseTask()
        {
            var courseId = Entity.Components.Get<CourseComponent>().CourseId;
            if (courseId == GameId.ZERO) return null;
            return Game.Scheduler.GetTask(courseId);
        }

        public CourseTaskExecutor? TryGetCourseTaskExecutor()
        {
            return TryGetCourseTask()?.Executor as CourseTaskExecutor;
        }
    }
}
