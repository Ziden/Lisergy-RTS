using Game.Events;
using Game.Events.Bus;
using Game.Events.ServerEvents;
using Game.Movement;
using Game.Network.ClientPackets;
using Game.Party;
using Game.Pathfinder;
using Game.Tile;
using Game.World;
using System.Collections.Generic;

namespace Game.Services
{
    public class CourseService : IEventListener
    {
        private GameWorld _world;

        public CourseService(StrategyGame _game)
        {
            this._world = _game.World;
            _game.NetworkEvents.Register<MoveRequestPacket>(this, RequestMovement);
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

            var first = ev.Path[0];
            if (_world.GetTile(first.X, first.Y).Distance(party.Tile) > 1)
            {
                ev.Sender.Send(new MessagePopupPacket(PopupType.BAD_INPUT));
                return;
            }

            var course = StartCourse(party, ev.Path, ev.Intent);
            if (course == null)
                ev.Sender.Send(new MessagePopupPacket(PopupType.BAD_INPUT));
        }

        private CourseTask StartCourse(PartyEntity party, List<Position> sentPath, MovementIntent intent)
        {
            List<TileEntity> path = new List<TileEntity>();
            var owner = party.Owner;

            foreach (var position in sentPath)
            {
                var tile = _world.GetTile(position.X, position.Y);
                if (!tile.Passable)
                {
                    Log.Error($"Impassable TileEntity {tile} in course path: {owner} moving party {party}");
                    return null;
                }
                path.Add(tile);
            }

            party.Course = new CourseTask(party, path, intent);
            return party.Course;
        }
    }
}
