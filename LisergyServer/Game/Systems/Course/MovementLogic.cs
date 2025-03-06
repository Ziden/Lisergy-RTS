using Game.Engine.DataTypes;
using Game.Engine.ECLS;
using Game.Engine.Events;
using Game.Engine.Scheduler;
using Game.Systems.Course;
using Game.Tile;
using Game.World;
using System;
using System.Collections.Generic;

namespace Game.Systems.Movement
{
    public unsafe class MovementLogic : BaseEntityLogic<MovementComponent>
    {
        public bool TryStartMovement(List<Location> sentPath, CourseIntent intent)
        {
            var owner = Game.Players.GetPlayer(CurrentEntity.OwnerID);
            foreach (var position in sentPath)
            {
                var tile = Game.World.GetTile(position.X, position.Y);
                if (!tile.Logic.Tile.IsPassable())
                {
                    Game.Log.Error($"Impassable TileEntity {tile} in course path: {owner} moving {CurrentEntity}");
                    return false;
                }
            }
            var movement = CurrentEntity.Components.Get<MovementComponent>();
            var taskExecutor = new CourseTaskExecutor(CurrentEntity, sentPath, intent);
            var task = new GameTask(Game, TimeSpan.FromMilliseconds(1), owner, taskExecutor);
            Game.Scheduler.Add(task);
            movement.CourseId = task.ID;
            movement.MovementIntent = intent;
            var ev = EventPool<CourseStartEvent>.Get();
            ev.Entity = CurrentEntity;
            ev.Intent = movement.MovementIntent;
            CurrentEntity.Save(movement);
            CurrentEntity.Components.CallEvent(ev);
            EventPool<CourseStartEvent>.Return(ev);
            return true;
        }

        public void FinishCourse(TileModel lastTile)
        {
            var movement = CurrentEntity.Components.Get<MovementComponent>();
            movement.CourseId = GameId.ZERO;
            var ev = EventPool<CourseFinishEvent>.Get();
            ev.Entity = CurrentEntity;
            ev.Intent = movement.MovementIntent;
            ev.ToTile = lastTile;
            CurrentEntity.Save(movement);
            CurrentEntity.Components.CallEvent(ev);
            EventPool<CourseFinishEvent>.Return(ev);
        }

        public GameTask GetCourseTask() => Game.Scheduler.GetTask(CurrentEntity.Components.Get<MovementComponent>().CourseId);

        public GameTask? TryGetCourseTask()
        {
            var courseId = CurrentEntity.Components.Get<MovementComponent>().CourseId;
            if (courseId == GameId.ZERO) return null;
            return Game.Scheduler.GetTask(courseId);
        }

        public CourseTaskExecutor? TryGetCourseTaskExecutor()
        {
            return TryGetCourseTask()?.Executor as CourseTaskExecutor;
        }
    }
}
