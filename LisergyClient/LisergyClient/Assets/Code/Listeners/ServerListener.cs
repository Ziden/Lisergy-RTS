using Assets.Code.World;
using Game;
using Game.Events;
using Game.Events.ServerEvents;
using Game.World;

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
            EventSink.OnUnitVisible += UnitVisible;
        }

        public void UnitVisible(UnitVisibleEvent ev)
        {
            using (new StackLog($"[Unit] Viewing {ev.Unit.SpecID} from {ev.Unit.OwnerID}"))
            {
                Unit u;
                if (!_world.Units.TryGetValue(ev.Unit.Id, out u))
                {
                    var tile = _world.GetTile(ev.Unit.X, ev.Unit.Y);
                    var clientUnit = new ClientUnit(ev.Unit);
                    var owner = clientUnit.Owner;
                 
                    tile.TeleportUnit(clientUnit);
                    StackLog.Debug("New Unit Created");
                } else
                {
                    StackLog.Debug("Unit Already Existed");
                }
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
                if(_world==null)
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
