using Game.Engine.DataTypes;
using Game.Engine.Network;
using Game.Events.ServerEvents;
using Game.Systems.Battler;
using Game.Systems.Movement;
using Game.World;
using System;
using System.Collections.Generic;

namespace Game.Systems.Course
{
    [Serializable]
    public class MoveEntityCommand : BasePacket, IGameCommand
    {
        public CourseIntent Intent;
        public GameId Entity;
        public List<Location> Path;

        public void Execute(IGame _game)
        {
            var ev = this;
            if (!_game.Entities.IsParent(ev.Sender.EntityId, ev.Entity))
            {
                _game.Log.Error($"{ev.Sender} cannot move entity {ev.Entity}");
                return;
            }
            var party = _game.Entities[ev.Entity];

            _game.Log.Debug($"{ev.Sender} requesting {ev.Entity} to move {ev.Path.Count} tiles");

            if (party.Get<BattleGroupComponent>().BattleID != GameId.ZERO)
            {
                _game.Network.SendToPlayer(new MessagePacket(MessageType.BAD_INPUT, "Bad move"), ev.Sender.EntityId);
                return;
            }

            var existingCourse = party.Logic.Movement.TryGetCourseTask();
            if (existingCourse != null)
            {
                _game.Scheduler.Cancel(existingCourse);
                _game.Log.Debug($"{party} cancelled previous course {existingCourse} with a new request");
            }

            var first = ev.Path[0];
            if (_game.World.GetTile(first.X, first.Y).Distance(party.Logic.Map.GetTile()) > 1)
            {
                _game.Network.SendToPlayer(new MessagePacket(MessageType.BAD_INPUT, "Bad route"), ev.Sender.EntityId);
                return;
            }

            if (!party.Logic.Movement.TryStartMovement(ev.Path, ev.Intent))
            {
                _game.Network.SendToPlayer(new MessagePacket(MessageType.BAD_INPUT, "Bad course"), ev.Sender.EntityId);
            }
        }
    }

}
