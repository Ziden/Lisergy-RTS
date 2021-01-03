using Assets.Code.World;
using Game;
using Game.Entity;
using Game.Events;
using Game.Events.ServerEvents;

namespace Assets.Code
{
    public class ServerListener
    {
        private ClientWorld _world;

        public ServerListener()
        {
            EventSink.OnSpecResponse += ReceiveSpecs;
        }

        public void RegisterGameHandlers()
        {
            EventSink.OnTileVisible += ReceiveTile;
            EventSink.OnPartyVisible += PartyVisible;
        }

        public void PartyVisible(PartyVisibleEvent ev)
        {
            using (new StackLog($"[Party] Viewing {ev.Party.PartyID} from {ev.Party.OwnerID}"))
            {
                var owner = _world.GetOrCreateClientPlayer(ev.Party.OwnerID);
                var tile = _world.GetTile(ev.Party.X, ev.Party.Y);
                var pt = new ClientParty(ev.Party);
                owner.Parties[ev.Party.PartyID] = pt;
                tile.TeleportParty(pt);
                pt.Render();
            }
        }

        public void ReceiveTile(TileVisibleEvent ev)
        {
            using (new StackLog("[TILE] Viewing " + ev.Tile))
            {
                var newTile = ev.Tile;
                var tile = (ClientTile)_world.GetTile(newTile.X, newTile.Y);
                if (ev.Tile.BuildingID != tile.BuildingID)
                {
                    if (ev.Tile.BuildingID == 0)
                        tile.Building = null;
                    else
                    {
                        var owner = _world.GetOrCreateClientPlayer(ev.Tile.UserID);
                        tile.Building = new ClientBuilding(ev.Tile.BuildingID, owner);

                        // Whenever we receive our main building focus camera on it
                        if(ev.Tile.BuildingID == StrategyGame.Specs.InitialBuilding)
                        {
                            CameraBehaviour.FocusOnTile(tile);
                        }

                    }
                }
                else
                    StackLog.Debug($"Same building id {tile.BuildingID}");
                tile.TileId = ev.Tile.TileId;
                tile.ResourceID = ev.Tile.ResourceID;
                tile.UpdateVisibility();
            }
        }

        public void ReceiveSpecs(GameSpecResponse ev)
        {
            using (new StackLog($"[Specs] V {ev.Spec.Version} Received {MainBehaviour.Player}"))
            {
                if (_world == null)
                {
                    _world = new ClientWorld();
                    _world.CreateWorld(ev.Cfg.WorldMaxPlayers);
                    MainBehaviour.StrategyGame = new ClientStrategyGame(ev.Cfg, ev.Spec, _world);
                    RegisterGameHandlers();
                }
            }
        }
    }
}
