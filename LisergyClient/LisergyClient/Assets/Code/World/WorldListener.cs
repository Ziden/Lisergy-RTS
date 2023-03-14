using Game;
using Game.Events.Bus;
using Game.Events.ServerEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Code.World
{
    public class WorldListener : IEventListener
    {

        public WorldListener(EventBus networkEvents)
        {
            networkEvents.Register<TileUpdatePacket>(this, TileUpdate);
        }

        [EventMethod]
        public void TileUpdate(TileUpdatePacket ev)
        {
            Log.Debug("Received tile");
            var tile = ClientStrategyGame.ClientWorld.GetClientTile(ev.Tile.X, ev.Tile.Y);
            tile.UpdateFrom(ev.Tile);
        }
    }
}
