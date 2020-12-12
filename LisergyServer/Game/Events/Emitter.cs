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
            Log.Debug($"Received {eventId.ToString()} event");
            if(eventId == EventID.AUTH_RESULT)
            {
                var ev = Serialization.ToEvent<AuthResultEvent>(message);
                EventSink.AuthResult(ev);
            } else if(eventId == EventID.JOIN)
            {
                var ev = Serialization.ToEvent<JoinWorldEvent>(message);
                ev.Player = owner;
                EventSink.JoinWorld(ev);
            }
        } 
    }
}
