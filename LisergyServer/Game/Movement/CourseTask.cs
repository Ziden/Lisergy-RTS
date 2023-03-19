using Game.Entity;
using Game.Entity.Components;
using Game.Scheduler;
using System.Collections.Generic;

namespace Game.Movement
{
    public class CourseTask : GameTask
    {
        public Party Party;
        List<Tile> Path;
        public MovementIntent Intent { get; private set; }

        public CourseTask(Party party, List<Tile> path, MovementIntent intent) : base(party.Components.Get<EntityMovementComponent>().MoveDelay, party.Owner)
        {
            this.Party = party;
            this.Path = path;
            this.Intent = intent;
        }

        public override void Execute()
        {
            var course = Party.Components.Get<EntityMovementComponent>().Course;
            if (course != this)
            {
                Repeat = false;
                Log.Error($"Party {Party} Had Course {course} but course {this} was trying to move the party");
                return;
            }
            this.Party.Tile = NextTile;
            Path.RemoveAt(0);
            Repeat = Path.Count > 0;   
        }

        public bool IsLastMovement()
        {
            return Path == null || Path.Count <= 1;
        }

        private Tile NextTile { get => Path == null || Path.Count == 0 ? null : Path[0]; }

        public override string ToString()
        {
            return $"<Course {ID.ToString()} Start=<{Start}> End={Finish} Party={Party} Next={NextTile}>";
        }
    }
}
