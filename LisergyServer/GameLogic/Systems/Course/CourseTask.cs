using System;
using System.Collections.Generic;
using System.Linq;
using Game.Engine.DataTypes;
using Game.Engine.ECLS;
using Game.Engine.Scheduler;
using Game.World;

namespace Game.Systems.Movement
{
	[Serializable]
	public class CourseTaskExecutor : ITaskExecutor
	{
		public GameId EntityId;
		public CourseIntent Intent;
		public List<Location> Path;

		public CourseTaskExecutor(IEntity party, Location[] path, CourseIntent intent)
		{
			EntityId = party.EntityId;
			Path = path.ToList();
			Intent = intent;
		}

		public void Execute(GameTask task)
		{
			var entity = task.Game.Entities[EntityId];
			task.Delay = entity.Components.Get<MovespeedComponent>().MoveDelay;
			var courseId = entity.Components.Get<MovementComponent>().CourseId;
			var currentCourse = task.Game.Scheduler.GetTask(courseId);
			if (currentCourse == null) return;
			if (currentCourse.Executor != this)
			{
				if (currentCourse.Start <= task.Start)
				{
					task.Game.Scheduler.Cancel(currentCourse);
				}
				else
				{
					task.Repeat = false;
					task.Game.Log.Error(
						$"Party {entity} Had Course {currentCourse} but course {this} was trying to move the party");
					return;
				}
			}

			var nextTile = Path == null || Path.Count == 0 ? null : task.Game.World.GetTile(Path[0].X, Path[0].Y);
			entity.Logic.Map.SetPosition(nextTile);
			Path.RemoveAt(0);
			task.Repeat = Path.Count > 0;
			if (!task.Repeat) entity.Logic.Movement.FinishCourse(nextTile);
		}

		public bool IsLastMovement()
		{
			return Path == null || Path.Count <= 1;
		}

		public override string ToString()
		{
			return $"<CourseExecutor Entity={EntityId} PathSize={Path?.Count}>";
		}
	}
}