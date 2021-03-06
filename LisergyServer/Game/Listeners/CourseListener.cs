using Game;
using Game.Entity;
using Game.Events;
using Game.World;
using System.Collections.Generic;

namespace Game.Listeners
{
    public class CourseListener : EventListener
    {
        private GameWorld _world;

        public CourseListener(GameWorld world)
        {
            this._world = world;
        }

        public override void Register()
        {
            NetworkEvents.OnEntityRequestMove += RequestMovement;
        }

        public override void Unregister()
        {
            NetworkEvents.OnEntityRequestMove -= RequestMovement;
        }

        public void RequestMovement(MoveRequestEvent ev)
        {
            var party = ev.Sender.Parties[ev.PartyIndex];
            Log.Debug($"{ev.Sender} requesting party {ev.PartyIndex} to move {ev.Path.Count} tiles");
            var course = StartCourse(party, ev.Path);
        }

        public CourseTask StartCourse(Party party, List<Position> sentPath)
        {
            List<Tile> path = new List<Tile>();
            var owner = party.Owner;
            foreach (var position in sentPath)
            {
                var tile = _world.GetTile(position.X, position.Y);
                if (!tile.Passable)
                {
                    Log.Error($"Impassable tile {tile} in course path: {owner} moving party {party}");
                    return null;
                }
                Log.Debug("ADD " + tile);
                path.Add(tile);
            }
            party.Course = new CourseTask(party, path);
            return party.Course;
        }

       
    }
}
