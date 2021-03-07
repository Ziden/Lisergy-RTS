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

        public void RegisterGameListeners()
        {
            NetworkEvents.OnTileVisible += ReceiveTile;
            NetworkEvents.OnEntityVisible += EntityVisible;
            NetworkEvents.OnEntityMove += EntityMove;
            NetworkEvents.OnMessagePopup += Message;
        }

        public void Message(MessagePopupEvent ev)
        {

        }

        public void EntityMove(EntityMoveEvent ev)
        {
            using (new StackLog($"[Entity] Moving {ev.ID}  from {ev.OwnerID} to {ev.X} {ev.Y}"))
            {
                var owner = _game.GetWorld().GetOrCreateClientPlayer(ev.OwnerID);
                var knownEntities = owner.KnownOwnedEntities;
                WorldEntity entity;
                if (!knownEntities.TryGetValue(ev.ID, out entity))
                    throw new System.Exception($"Server sent move event for entity {ev.ID} from {ev.OwnerID} at {ev.X}-{ev.Y} however its not visible to client");
                var newTile = (ClientTile)_game.GetWorld().GetTile(ev.X, ev.Y);
                entity.Tile = newTile;
            }
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
                tile.SetVisible(true);
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
                    RegisterGameListeners();
                }
            }
        }
    }
}
