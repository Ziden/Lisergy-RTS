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

            // CLIENT
            NetworkEvents.OnJoinWorld += JoinWorld;
            NetworkEvents.OnPartyRequestMove += PartyRequestMove;

            // SERVER
            NetworkEvents.OnTileVisible += TileVisible;
            NetworkEvents.OnPartyVisible += PartyVisible;

            Log.Debug("World Event Listener Registered");
        }

        public void PartyRequestMove(MoveRequestEvent ev)
        {
            var player = ev.ClientPlayer;
            var party = player.Parties[ev.PartyIndex];
            var path = ev.Path;
        }

        public void PartyVisible(PartyVisibleEvent ev)
        {
            ev.Viewer.Owner.Send(ev);
            Log.Debug($"{ev.Viewer.Owner} sees {ev.Party}");
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
                    foreach(var unit in tile.Parties)
                    {
                        Log.Debug($"Sending unit {tile}");
                        player.Send(new PartyVisibleEvent() { Party = unit });
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
