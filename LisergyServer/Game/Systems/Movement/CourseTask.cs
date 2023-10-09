using Game.ECS;
using Game.Scheduler;
using Game.Systems.Party;
using Game.Tile;
using System.Collections.Generic;

namespace Game.Systems.Movement
{
    public class CourseTask : GameTask
    {
        public IEntity Party;
        public List<TileEntity> Path;
        public MovementIntent Intent { get; private set; }

        public CourseTask(IGame game, IEntity party, List<TileEntity> path, MovementIntent intent) : base(game, party.Components.Get<EntityMovementComponent>().MoveDelay, game.Players.GetPlayer(party.OwnerID))
        {
            Party = party;
            Path = path;
            Intent = intent;
        }

        public override void Tick()
        {
            var courseId = Party.Components.Get<EntityMovementComponent>().CourseId;
            var currentCourse = Game.Scheduler.GetTask(courseId);
            if (currentCourse != this)
            {
                if(currentCourse.Start <= Start)
                {
                    Game.Scheduler.Cancel(currentCourse);
                } else
                {
                    Repeat = false;
                    Log.Error($"Party {Party} Had Course {currentCourse} but course {this} was trying to move the party");
                    return;
                }
            }
            Game.Systems.Map.GetLogic(Party).SetPosition(NextTile);
            Path.RemoveAt(0);
            Repeat = Path.Count > 0;
            if(!Repeat)
            {
                Game.Systems.EntityMovement.GetLogic(Party).FinishCourse();
            }
        }

        public bool IsLastMovement()
        {
            return Path == null || Path.Count <= 1;
        }

        private TileEntity NextTile { get => Path == null || Path.Count == 0 ? null : Path[0]; }

        public override string ToString()
        {
            return $"<Course {ID.ToString()} Start=<{Start}> End={Finish} Party={Party} Next={NextTile}>";
        }
    }
}
