using Game;
using Game.Events;
using Game.Events.ServerEvents;
using System.Linq;

namespace LisergyServer
{
    public class ServerWorldListener
    {
        private GameWorld _world;

        public ServerWorldListener(GameWorld w)
        {
            _world = w; 
            EventSink.OnJoinWorld += JoinWorld;
            EventSink.OnTileVisible += TileVisible;
            EventSink.OnUnitVisible += UnitVisible;
            Log.Debug("World Event Listener Registered");
        }

        public void UnitVisible(UnitVisibleEvent ev)
        {
            ev.Viewer.Owner.Send(ev);
            Log.Debug($"{ev.Viewer.Owner} sees {ev.Unit}");
        }

        public void TileVisible(TileVisibleEvent ev)
        {
            ev.Viewer.Owner.Send(ev);
            Log.Debug($"{ev.Viewer.Owner} sees {ev.Tile}");
        }

        public void JoinWorld(JoinWorldEvent ev)
        {
            PlayerEntity player = null;
            if (_world.Players.GetPlayer(ev.ClientPlayer.UserID, out player))
            {
                Log.Debug($"Existing player {player.UserID} joined");
                Log.Debug($"Sending {player.VisibleTiles.Count} visible tiles");
                foreach (var tile in player.VisibleTiles)
                {
                    Log.Debug($"Sending tile {tile}");
                    player.Send(new TileVisibleEvent() { Tile = tile });
                    foreach(var unit in tile.Units)
                    {
                        Log.Debug($"Sending unit {tile}");
                        player.Send(new UnitVisibleEvent() { Unit = unit });
                    }
                }
            }
            else
            {
                player = ev.ClientPlayer;
                _world.PlaceNewPlayer(player);
                Log.Debug($"New player {player.UserID} joined the world");

                /*
                foreach (var tile in _world.AllTiles())
                    if(!tile.IsVisibleTo(player))
                        tile.SetSeenBy(player.Buildings.First());
                */

            }
        }
    }
}
