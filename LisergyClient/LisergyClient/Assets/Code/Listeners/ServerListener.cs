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
            using (new StackLog($"[Entity] Viewing {ev.Entity} from {ev.Entity.OwnerID}"))
            {
                var owner = _game.GetWorld().GetOrCreateClientPlayer(ev.Entity.OwnerID);
                var tile = (ClientTile)_game.GetWorld().GetTile(ev.Entity.X, ev.Entity.Y);
                var clientEntity = EntityFactory.InstantiateClientEntity(ev.Entity, owner, tile);
                clientEntity.Tile = tile;
            }
            UIManager.PartyUI.RenderAllParties();
        }

        public void ReceiveTile(TileVisibleEvent ev)
        {
            using (new StackLog("[TILE] Viewing " + ev.Tile))
            {
                var newTile = ev.Tile;
                var tile = (ClientTile)_game.GetWorld().GetTile(newTile.X, newTile.Y);
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
