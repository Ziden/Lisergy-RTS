using Game.Events;
using Game.Events.ServerEvents;
using System.Linq;

namespace Game
{
    public class WorldListener
    {
        private GameWorld _world;

        public WorldListener(GameWorld w)
        {
            _world = w;
            RegisterEvents();
            Log.Debug("World Event Listener Registered");
        }

        public virtual void RegisterEvents()
        {
            EventSink.OnJoinWorld += JoinWorld;
            EventSink.OnTileVisible += TileVisible;
        }

        public void TileVisible(TileVisibleEvent ev)
        {
            Log.Debug($"View tile " + ev.Tile);
            ev.Viewer.Owner.Send(ev);
        }

        public void JoinWorld(JoinWorldEvent ev)
        {
            PlayerEntity player = null;
            if(_world.Players.GetPlayer(ev.ClientPlayer.UserID, out player))
            {
                Log.Debug($"Existing player {player.UserID} joined");
                Log.Debug($"Sending {player.VisibleTiles.Count} visible tiles");
                foreach(var tile in player.VisibleTiles)
                {
                    Log.Debug($"Sending tile {tile}");
                    player.Send(new TileVisibleEvent() { Tile = tile });
                }     
            } else
            {
                player = ev.ClientPlayer;
                _world.AddPlayer(player);
                Log.Debug($"New player {player.UserID} joined the world");

                foreach (var tile in _world.AllTiles())
                    if(!tile.IsVisibleTo(player))
                        tile.SetSeenBy(player.Buildings.First());
            }
        }
    }
}
