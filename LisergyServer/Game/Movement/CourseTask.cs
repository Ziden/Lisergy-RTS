using Game.Entity;
using Game.Scheduler;
using System;
using System.Collections.Generic;

namespace Game.World
{
    public class CourseTask : GameTask
    {
        public Party Party;
        List<Tile> Path;

        public CourseTask(Party party, List<Tile> path): base(party.GetMoveDelay())
        {
            this.Party = party;
            this.Path = path;
        }

        public override void Execute()
        {
            if (Party.Course != this)
            {
                Log.Error($"Party {Party} Had Course {Party.Course} but course {this} was trying to move the party");
                return;
            }
            this.Party.Tile = NextTile;
            Path.RemoveAt(0);
            Repeat = Path.Count > 0;   
        }

        private Tile NextTile { get => Path == null || Path.Count == 0 ? null : Path[0]; }

        public override string ToString()
        {
            return $"<Course {ID.ToString()} Start=<{Start}> End={Finish} Party={Party} Next={NextTile}>";
        }
    }
}
