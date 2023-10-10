using Game.Events.Bus;
using Game.Events.ServerEvents;
using Game.Network.ClientPackets;
using Game.Systems.MapPosition;
using Game.Systems.Movement;
using Game.Systems.Party;
using Game.Tile;
using Game.World;
using System.Collections.Generic;

namespace Game.Services
{
    public class CourseService : IEventListener
    {
        private GameWorld _world;
        private IGame _game;

        public CourseService(LisergyGame game)
        {
            _game = game;
            this._world = game.World;
            _game.Network.On<MoveRequestPacket>(RequestMovement);
        }

        public void RequestMovement(MoveRequestPacket ev)
        {
            var party = ev.Sender.GetParty(ev.PartyIndex);
            Log.Debug($"{ev.Sender} requesting party {ev.PartyIndex} to move {ev.Path.Count} tiles");

            if (!party.CanMove())
            {
                ev.Sender.SendMessage("Bad move");
                return;
            }

            var existingCourse = party.EntityLogic.Movement.TryGetCourseTask();
            if(existingCourse != null)
            {
                _game.Scheduler.Cancel(existingCourse);
                Log.Info($"{party} cancelled previous course {existingCourse} with a new request");
            }

            var first = ev.Path[0];
            if (_world.GetTile(first.X, first.Y).Distance(party.Tile) > 1)
            {
                ev.Sender.SendMessage("Bad route");
                return;
            }

            if (!party.EntityLogic.Movement.TryStartMovement(ev.Path, ev.Intent))
            {
                ev.Sender.SendMessage("Bad course");
            } else
            {
                var s = _game.Scheduler;
                var asd = 123;
            }
        }
    }
}
