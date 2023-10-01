using Game.Scheduler;
using Game.Systems.Party;
using Game.Tile;
using System.Collections.Generic;

namespace Game.Systems.Movement
{
    public class CourseTask : GameTask
    {
        public PartyEntity Party;
        List<TileEntity> Path;
        public MovementIntent Intent { get; private set; }

        public CourseTask(PartyEntity party, List<TileEntity> path, MovementIntent intent) : base(party.Components.Get<EntityMovementComponent>().MoveDelay, party.Owner)
        {
            Party = party;
            Path = path;
            Intent = intent;
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
            Party.Tile = NextTile;
            Path.RemoveAt(0);
            Repeat = Path.Count > 0;
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
