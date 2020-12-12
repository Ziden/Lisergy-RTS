using Game.Events;
using Game.Events.ServerEvents;

namespace Game
{
    public class WorldListener
    {
        private GameWorld _world;

        public WorldListener(GameWorld w)
        {
            _world = w;
            EventSink.OnJoinWorld += JoinWorld;
            EventSink.OnTileVisible += TileVisible;
            Log.Debug("World Event Listener Registered");
        }

        public void TileVisible(TileVisibleEvent ev)
        {
            ev.Viewer.Owner.Send(ev);
        }

        public void JoinWorld(JoinWorldEvent ev)
        {
            PlayerEntity player = null;
            if(_world.Players.GetPlayer(ev.Player.UserID, out player))
            {
                Log.Debug($"Existing player {player.UserID} joined");
            } else
            {
                player = ev.Player;
                _world.AddPlayer(player);
                Log.Debug($"New player {player.UserID} joined the world");
            }
        }
    }
}
