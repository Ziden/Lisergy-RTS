using Game.DataTypes;
using Game.ECS;
using Game.Scheduler;
using Game.Systems.Party;
using Game.Tile;
using Game.World;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

namespace Game.Systems.Movement
{
    public class EntityMovementLogic : BaseEntityLogic<EntityMovementComponent>
    {
        public bool TryStartMovement(List<Position> sentPath, MovementIntent intent)
        {
            Log.Debug("Validating route");
            var owner = Game.Players.GetPlayer(Entity.OwnerID);

            foreach (var position in sentPath)
            {
                var tile = Game.GameWorld.GetTile(position.X, position.Y);
                if (!tile.Passable)
                {
                    Log.Error($"Impassable TileEntity {tile} in course path: {owner} moving {Entity}");
                    return false;
                }
            }
            var movement = Entity.Get<EntityMovementComponent>();
            var task = new CourseTask(Game, Entity, sentPath, intent);
            movement.CourseId = task.ID;
            movement.MovementIntent = intent;
            Entity.Components.Save(movement);
            return true;
        }

        public void FinishCourse()
        {
            var movement = Entity.Get<EntityMovementComponent>();
            movement.CourseId = GameId.ZERO;
            Entity.Components.Save(movement);
        }

        public GameTask GetCourse() => Game.Scheduler.GetTask(Entity.Components.Get<EntityMovementComponent>().CourseId);

        public CourseTask? TryGetCourseTask()
        {
            if(Entity.Components.Get<EntityMovementComponent>().CourseId == GameId.ZERO) return null;
            return (CourseTask)Game.Scheduler.GetTask(Entity.Components.Get<EntityMovementComponent>().CourseId);
        }

        public void SetCourse(CourseTask newCourse)
        {
            var existingCourse = GetCourse();
            if (existingCourse != null && !existingCourse.HasFinished)
                existingCourse.Cancel();
            var component = Entity.Components.Get<EntityMovementComponent>();
            component.CourseId = newCourse.ID;
            Entity.Components.Save(component);
        }
    }
}
