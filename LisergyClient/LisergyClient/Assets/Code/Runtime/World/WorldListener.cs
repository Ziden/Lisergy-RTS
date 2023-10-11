using Game.Events.Bus;
using Game.Events.ServerEvents;
using Game.Network;

namespace Assets.Code.World
{
    public class WorldListener : IEventListener
    {
        public WorldListener(EventBus<BasePacket> networkEvents)
        {
            networkEvents.Register<TilePacket>(this, TileUpdate);
        }

        public void TileUpdate(TilePacket ev)
        {
            var tile = GameView.World.GetTile(ev.Data.Position);
            GameView.GetOrCreateTileView(tile).UpdateFromData(ev.Data);
        }
    }
}
