using Game;
using Game.Events;
using Game.Events.ServerEvents;

namespace Game.Listeners
{
    public class WorldListener : EventListener
    {
        private GameWorld _world;

        public WorldListener(GameWorld w)
        {
            _world = w; 
            Log.Debug("World Event Listener Registered");
        }

        public override void Register()
        {
            NetworkEvents.OnJoinWorld += JoinWorld;
        }

        public override void Unregister()
        {
            NetworkEvents.OnJoinWorld -= JoinWorld;
        }

        public void JoinWorld(JoinWorldEvent ev)
        {
            PlayerEntity player = null;
            if (_world.Players.GetPlayer(ev.Sender.UserID, out player))
            {
                Log.Debug($"Existing player {player.UserID} joined");
                Log.Debug($"Sending {player.VisibleTiles.Count} visible tiles");
                foreach (var tile in player.VisibleTiles)
                {
                    Log.Debug($"Sending tile {tile}");
                    tile.SendTileInformation(player);
                }
            }
            else
            {
                player = ev.Sender;
                _world.PlaceNewPlayer(player);
                Log.Debug($"New player {player.UserID} joined the world");
            }
        }

    }
}
