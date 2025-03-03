using Game.Engine.Network;
using Game.Systems.Player;
using System;

namespace Game.Systems.Map
{
    [Serializable]
    public class JoinWorldMapCommand : BasePacket, IGameCommand
    {
        public void Execute(IGame game)
        {
            var _world = game.World;
            var ev = this;

            if (_world.Players.GetPlayer(ev.SenderPlayerId, out var player))
            {
                _world.Game.Log.Debug($"Existing player {player.EntityId} joined");
                player.PlayerEntity.Logic.DeltaCompression.SendAllVisibleTiles();
            }
            else
            {
                _world.Game.Log.Debug($"New player {ev.SenderPlayerId} joined the world");
                var playerEntity = ev.Sender?.PlayerEntity ?? _world.Game.Entities[ev.SenderPlayerId];
                var playerModel = ev.Sender ?? new PlayerModel(_world.Game, playerEntity);
                _world.Players.Add(playerModel);
                playerEntity.Logic.Player.PlaceNewPlayer(_world.GetUnusedStartingTile());
            }
        }
    }
}
