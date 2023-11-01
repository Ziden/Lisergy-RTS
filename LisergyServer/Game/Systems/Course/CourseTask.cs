using Game.ECS;
using Game.Scheduler;
using Game.Systems.Party;
using Game.Tile;
using Game.World;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Game.DataTypes;

namespace Game.Systems.Movement
{
    [Serializable]
    public unsafe class CourseTaskExecutor : ITaskExecutor
    {
        public GameId EntityId;
        public List<TileVector> Path;
        public CourseIntent Intent;

        public CourseTaskExecutor(IEntity party, List<TileVector> path, CourseIntent intent)
        {
            EntityId = party.EntityId;
            Path = path;
            Intent = intent;
        }

        public void Execute(GameTask task)
        {
            var entity = task.Game.Entities[EntityId];
            task.Delay = entity.Components.Get<MovespeedComponent>().MoveDelay;
            var courseId = entity.Components.Get<CourseComponent>().CourseId;
            var currentCourse = task.Game.Scheduler.GetTask(courseId);
            if (currentCourse == null) return;
            if (currentCourse.Executor != this)
            {
                if(currentCourse.Start <= task.Start)
                {
                    task.Game.Scheduler.Cancel(currentCourse);
                } else
                {
                    task.Repeat = false;
                    task.Game.Log.Error($"Party {entity} Had Course {currentCourse} but course {this} was trying to move the party");
                    return;
                }
            }

            var nextTile = Path == null || Path.Count == 0 ? null : task.Game.World.Map.GetTile(Path[0].X, Path[0].Y);
            task.Game.Systems.Map.GetLogic(entity).SetPosition(nextTile);
            Path.RemoveAt(0);
            task.Repeat = Path.Count > 0;
            if(!task.Repeat)
            {
                task.Game.Systems.EntityMovement.GetLogic(entity).FinishCourse(nextTile);
            }
        }

        public bool IsLastMovement() => Path == null || Path.Count <= 1;
        public override string ToString() => $"<CourseExecutor Entity={EntityId} PathSize={Path?.Count}>";
    }
}
