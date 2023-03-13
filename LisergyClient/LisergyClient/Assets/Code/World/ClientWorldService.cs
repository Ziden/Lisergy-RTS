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
    public class ClientWorldService : IEventListener
    {
        ClientStrategyGame _game;

        public ClientWorldService(ClientStrategyGame game)
        {
            _game = game;
            _game.NetworkEvents.Register<TileUpdatePacket>(this, TileUpdate);
        }

        [EventMethod]
        public void TileUpdate(TileUpdatePacket ev)
        {
            Log.Debug("Received tile");
            var tile = _game.GetWorld().GetClientTile(ev.Tile.X, ev.Tile.Y);
            tile.UpdateFrom(ev.Tile);
        }
    }
}
