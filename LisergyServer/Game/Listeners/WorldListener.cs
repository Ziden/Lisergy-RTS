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
            // CLIENT
            NetworkEvents.OnJoinWorld += JoinWorld;

            // SERVER
            NetworkEvents.OnTileVisible += TileVisible;
            NetworkEvents.OnPartyVisible += PartyVisible;

        }

        public override void Unregister()
        {
            NetworkEvents.OnJoinWorld -= JoinWorld;
            NetworkEvents.OnTileVisible -= TileVisible;
            NetworkEvents.OnPartyVisible -= PartyVisible;

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
            if (_world.Players.GetPlayer(ev.Sender.UserID, out player))
            {
                Log.Debug($"Existing player {player.UserID} joined");
                Log.Debug($"Sending {player.VisibleTiles.Count} visible tiles");
                foreach (var tile in player.VisibleTiles)
                {
                    Log.Debug($"Sending tile {tile}");
                    player.Send(new TileVisibleEvent(tile));
                    foreach(var party in tile.Parties)
                    {
                        Log.Debug($"Sending unit {tile}");
                        player.Send(new PartyVisibleEvent(party, null));
                    }
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
