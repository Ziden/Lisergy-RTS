using Game.ECS;
using Game.Scheduler;
using Game.Systems.Party;
using Game.Tile;
using Game.World;
using System.Collections.Generic;

namespace Game.Systems.Movement
{
    public class CourseTask : GameTask
    {
        public IEntity Party;
        public List<Position> Path;
        public CourseIntent Intent { get; private set; }

        public CourseTask(IGame game, IEntity party, List<Position> path, CourseIntent intent) : base(game, party.Components.Get<MovespeedComponent>().MoveDelay, game.Players.GetPlayer(party.OwnerID))
        {
            Party = party;
            Path = path;
            Intent = intent;
        }

        public override void Tick()
        {
            var courseId = Party.Components.Get<CourseComponent>().CourseId;
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

        private TileEntity NextTile { get => Path == null || Path.Count == 0 ? null : Game.World.Map.GetTile(Path[0].X, Path[0].Y); }

        public override string ToString()
        {
            return $"<Course {ID.ToString()} Start=<{Start}> End={Finish} Party={Party} Next={NextTile}>";
        }
    }
}
