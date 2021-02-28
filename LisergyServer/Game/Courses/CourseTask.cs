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

        public CourseTask(Party party, List<Tile> path): base(party.MoveDelay)
        {
            this.Party = party;
            this.Path = path;
        }

        public override void Execute()
        {
            if (Party.Course != this)
                throw new Exception($"Party {Party} Had Course {Party.Course} but course {this} was trying to move the party");

            NextTile.TeleportParty(this.Party);
            Path.RemoveAt(0);
        }

        private Tile NextTile { get => Path.Count == 0 ? null : Path[0]; }

        public override string ToString()
        {
            return $"<Course {ID.ToString()} End={Finish} Party={Party} Next={NextTile}>";
        }
    }
}
