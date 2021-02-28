using Assets.Code.World;
using Game;
using Game.Entity;
using Game.Events;
using Game.Events.ServerEvents;

namespace Assets.Code
{
    public class ServerListener
    {
        private ClientStrategyGame _game;

        public ServerListener()
        {
            NetworkEvents.OnSpecResponse += ReceiveSpecs;
        }

        public void RegisterGameHandlers()
        {
            NetworkEvents.OnTileVisible += ReceiveTile;
            NetworkEvents.OnEntityVisible += EntityVisible;
        }

        public void EntityVisible(EntityVisibleEvent ev)
        {
            if(ev.Party is Party)
            {
                var party = (Party)ev.Party;
                using (new StackLog($"[Party] Viewing {party.PartyIndex} from {ev.Party.OwnerID}"))
                {
                    var owner = _game.GetWorld().GetOrCreateClientPlayer(ev.Party.OwnerID);
                    var tile = _game.GetWorld().GetTile(ev.Party.X, ev.Party.Y);
                    var pt = new ClientParty(owner, party);
                    owner.Parties[party.PartyIndex] = pt;
                    pt.Tile = tile;
                    pt.Render();
                }
                UIManager.PartyUI.RenderAllParties();
            }
        }

        public void ReceiveTile(TileVisibleEvent ev)
        {
            using (new StackLog("[TILE] Viewing " + ev.Tile))
            {
                var newTile = ev.Tile;
                var tile = (ClientTile)_game.GetWorld().GetTile(newTile.X, newTile.Y);
                if (ev.Tile.BuildingID != tile.BuildingID)
                {
                    if (ev.Tile.BuildingID == 0)
                        tile.Building = null;
                    else
                    {
                        var owner = _game.GetWorld().GetOrCreateClientPlayer(ev.Tile.UserID);
                        tile.Building = new ClientBuilding(ev.Tile.BuildingID, owner, tile);

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
                if (_game == null)
                {
                    var world = new ClientWorld();
                    world.CreateWorld(ev.Cfg.WorldMaxPlayers);
                    _game = new ClientStrategyGame(ev.Cfg, ev.Spec, world);
                    RegisterGameHandlers();
                }
            }
        }
    }
}
