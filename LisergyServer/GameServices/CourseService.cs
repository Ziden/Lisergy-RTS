using Game.Engine.DataTypes;
using Game.Engine.Events.Bus;
using Game.Network.ClientPackets;
using Game.Systems.Battler;
using Game.World;

namespace Game.Services
{
    public class CourseService : IEventListener
    {
        private IGame _game;

        public CourseService(LisergyGame game)
        {
            _game = game;
            _game.Network.On<MoveRequestPacket>(RequestMovement);
        }

        public void RequestMovement(MoveRequestPacket ev)
        {
            var party = ev.Sender.GetParty(ev.PartyIndex);
            _game.Log.Debug($"{ev.Sender} requesting party {ev.PartyIndex} to move {ev.Path.Count} tiles");

            if (party.Get<BattleGroupComponent>().BattleID != GameId.ZERO)
            {
                ev.Sender.SendMessage("Bad move");
                return;
            }

            var existingCourse = party.EntityLogic.Movement.TryGetCourseTask();
            if(existingCourse != null)
            {
                _game.Scheduler.Cancel(existingCourse);
                _game.Log.Debug($"{party} cancelled previous course {existingCourse} with a new request");
            }

            var first = ev.Path[0];
            if (_game.World.Map.GetTile(first.X, first.Y).Distance(party.Tile) > 1)
            {
                ev.Sender.SendMessage("Bad route");
                return;
            }

            if (!party.EntityLogic.Movement.TryStartMovement(ev.Path, ev.Intent))
            {
                ev.Sender.SendMessage("Bad course");
            } 
        }
    }
}
