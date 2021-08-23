using Game;
using Game.Events;
using Game.Events.Bus;
using Game.Events.GameEvents;
using Game.Events.ServerEvents;
using System.Linq;

namespace Game.Listeners
{
    public class WorldPacketListener : IEventListener
    {
        private GameWorld _world;

        public WorldPacketListener(GameWorld w)
        {
            _world = w; 
            Log.Debug("World Event Listener Registered");
        }

        [EventMethod]
        public void JoinWorld(JoinWorldPacket ev)
        {
            PlayerEntity player = null;
            if (_world.Players.GetPlayer(ev.Sender.UserID, out player))
            {
                Log.Debug($"Existing player {player.UserID} joined");
                _world.Game.GameEvents.Call(new PlayerJoinedEvent(player));
            }
            else
            {
                player = ev.Sender;
                _world.PlaceNewPlayer(player);
                Log.Debug($"New player {player.UserID} joined the world");
            }
        }
    }
}
