using Game;
using Game.Events;
using Game.Events.ServerEvents;
using Game.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Code.World
{
    class ClientWorldListener : WorldListener
    {
        private ClientWorld _world;

        public ClientWorldListener(ClientWorld world) : base(world)
        {
            this._world = world;
        }

        public override void RegisterEvents()
        {
            EventSink.OnTileVisible += OnTileVisible;
        }

        public void OnTileVisible(TileVisibleEvent ev)
        {
            Log.Debug("Received new tile visible " + ev.Tile);
            var newTile = ev.Tile;
            var tile = (ClientTile)_world.GetTile(newTile.X, newTile.Y);

            tile.TileId = ev.Tile.TileId;
            tile.ResourceID = ev.Tile.ResourceID;
            if (ev.Tile.BuildingID != tile.BuildingID)
            {
                if(ev.Tile.BuildingID == 0)
                {
                    tile.Building = null;
                } else
                {   
                    Log.Debug("Building " + ev.Tile.BuildingID);
                    var owner = _world.GetClientPlayer(ev.Tile.UserID);
                    tile.Building = new Building(ev.Tile.BuildingID, owner);
                }
            }
        }
    }
}
