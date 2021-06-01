using Game;
using Game.Events;
using Game.Events.Bus;
using Game.Events.ServerEvents;
using System.Linq;

namespace Game.Listeners
{
    public class WorldListener : IEventListener
    {
        private GameWorld _world;

        public WorldListener(GameWorld w)
        {
            _world = w; 
            Log.Debug("World Event Listener Registered");
        }

        [EventMethod]
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
                    tile.SendTileInformation(player, null);
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
