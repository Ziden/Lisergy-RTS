using Game;
using Game.Entity;
using Game.Events;
using Game.Events.Bus;
using Game.Events.ServerEvents;
using Game.Movement;
using Game.World;
using System.Collections.Generic;

namespace Game.Listeners
{
    public class CoursePacketListener : IEventListener
    {
        private GameWorld _world;

        public CoursePacketListener(GameWorld world)
        {
            this._world = world;
        }

        [EventMethod]
        public void RequestMovement(MoveRequestPacket ev)
        {
            var party = ev.Sender.GetParty(ev.PartyIndex);
            Log.Debug($"{ev.Sender} requesting party {ev.PartyIndex} to move {ev.Path.Count} tiles");

            if (!party.CanMove())
            {
                ev.Sender.Send(new MessagePopupPacket(PopupType.BAD_INPUT));
                return;
            }

            var course = StartCourse(party, ev.Path, ev.Intent);
            if(course == null)
                ev.Sender.Send(new MessagePopupPacket(PopupType.BAD_INPUT));
        }

        private CourseTask StartCourse(Party party, List<Position> sentPath, MovementIntent intent)
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
                path.Add(tile);
            }
            party.Course = new CourseTask(party, path, intent);
            return party.Course;
        }
    }
}
