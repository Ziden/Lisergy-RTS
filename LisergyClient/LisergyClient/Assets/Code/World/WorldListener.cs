using Assets.Code.Views;
using Game;
using Game.Events;
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

        public WorldListener(EventBus<ServerPacket> networkEvents)
        {
            networkEvents.Register<TileUpdatePacket>(this, TileUpdate);
        }

        [EventMethod]
        public void TileUpdate(TileUpdatePacket ev)
        {
            var tile = GameView.World.GetTile(ev.Data.X, ev.Data.Y);
            GameView.GetOrCreateTileView(tile).UpdateFromData(ev.Data);
        }
    }
}
