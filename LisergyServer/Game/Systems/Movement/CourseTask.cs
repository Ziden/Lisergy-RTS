using Game.Scheduler;
using Game.Systems.Party;
using Game.Tile;
using System.Collections.Generic;

namespace Game.Systems.Movement
{
    public class CourseTask : GameTask
    {
        public PartyEntity Party;
        public List<TileEntity> Path;

        public MovementIntent Intent { get; private set; }

        public CourseTask(IGame game, PartyEntity party, List<TileEntity> path, MovementIntent intent) : base(game, party.Components.Get<EntityMovementComponent>().MoveDelay, game.Players.GetPlayer(party.OwnerID))
        {
            Party = party;
            Path = path;
            Intent = intent;
        }

        public override void Tick()
        {
            var course = Party.Components.Get<EntityMovementComponent>().Course;
            if (course != this)
            {
                Repeat = false;
                Log.Error($"Party {Party} Had Course {course} but course {this} was trying to move the party");
                return;
            }
            Game.Systems.Map.GetEntityLogic(Party).SetPosition(NextTile);
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
