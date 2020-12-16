using Game;
using Game.Events;
using Game.Events.ServerEvents;

namespace LisergyServer.Core
{
    public class EventEmitter
    {
        public static void CallEventFromBytes(PlayerEntity owner, byte [] message)
        {
            var eventId = (EventID)message[0];
            Log.Debug($"Received {eventId.ToString()} event from network, calling handler");

            // SERVER EVENTS (from server to client/server)
            if(eventId == EventID.AUTH_RESULT)
            {
                var ev = Serialization.ToEvent<AuthResultEvent>(message);
                ev.ClientPlayer = owner;
                EventSink.AuthResult(ev);
            }
            else if (eventId == EventID.TILE_VISIBLE)
            {
                Log.Debug("Sending Visible Tile Event");
                var ev = Serialization.ToEvent<TileVisibleEvent>(message);
                ev.ClientPlayer = owner;
                EventSink.TileVisible(ev);
            }
            else if (eventId == EventID.SPEC_RESPONSE)
            {
                var ev = Serialization.ToEvent<GameSpecResponse>(message);
                ev.ClientPlayer = owner;
                EventSink.SpecResponse(ev);
            }

            // CLIENT EVENTS (coming from client to server)
            else if(eventId == EventID.JOIN)
            {
                var ev = Serialization.ToEvent<JoinWorldEvent>(message);
                ev.ClientPlayer = owner;
                EventSink.JoinWorld(ev);
            }
           
        } 
    }
}
