using Game.Events.Bus;
using Game.Network;
using Game.Network.ClientPackets;
using Game.Systems.Player;
using Game.Systems.World;

namespace Game.Services
{
    public class WorldService : IEventListener
    {
        private GameLogic _game;
        private GameWorld _world;

        public WorldService(GameLogic game)
        {
            _game = game;
            _world = game.World;
            game.NetworkPackets.Register<JoinWorldPacket>(this, JoinWorld);
        }

        [EventMethod]
        public void JoinWorld(JoinWorldPacket ev)
        {
            PlayerEntity player = null;
            if (_world.Players.GetPlayer(ev.Sender.UserID, out player))
            {
                Log.Debug($"Existing player {player.UserID} joined");
                foreach (var tile in player.VisibleTiles)
                {
                    tile.SetFlagIncludingChildren(DeltaFlag.SELF_REVEALED);
                }
            }
            else
            {
                var startTile = _world.GetUnusedStartingTile();
                _world.PlaceNewPlayer(ev.Sender, startTile);
                Log.Debug($"New player {ev.Sender.UserID} joined the world");
            }
        }
    }
}
