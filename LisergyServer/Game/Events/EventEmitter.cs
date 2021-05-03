using Game;
using Game.Events;
using Game.Events.ServerEvents;

namespace LisergyServer.Core
{
    public class EventEmitter
    {
        // TODO MAKE THIS WITH REFLECTION PLX
        public static void CallEventFromBytes(PlayerEntity owner, byte [] message)
        {
            var eventId = (EventID)message[0];
            Log.Debug($"Received {eventId.ToString()} event from network, calling handler");

            // SERVER EVENTS (from server to client/server)
            if(eventId == EventID.AUTH_RESULT)
            {
                var ev = Serialization.ToEvent<AuthResultEvent>(message);
                ev.Sender = owner;
                NetworkEvents.SendAuthResult(ev);
            }
            else if (eventId == EventID.TILE_VISIBLE)
            {
                var ev = Serialization.ToEvent<TileVisibleEvent>(message);
                ev.Sender = owner;
                NetworkEvents.SendTileVisible(ev);
            }
            else if (eventId == EventID.SPEC_RESPONSE)
            {
                var ev = Serialization.ToEvent<GameSpecResponse>(message);
                ev.Sender = owner;
                NetworkEvents.SendSpecResponse(ev);
            }
            else if (eventId == EventID.PARTY_VISIBLE)
            {
                var ev = Serialization.ToEvent<EntityVisibleEvent>(message);
                ev.Sender = owner;
                NetworkEvents.SendEntityVisible(ev);
            }
            else if (eventId == EventID.PARTY_MOVE)
            {
                var ev = Serialization.ToEvent<EntityMoveEvent>(message);
                ev.Sender = owner;
                NetworkEvents.SendEntityMove(ev);
            }
            else if (eventId == EventID.MESSAGE)
            {
                var ev = Serialization.ToEvent<MessagePopupEvent>(message);
                ev.Sender = owner;
                NetworkEvents.SendMessagePopup(ev);
            }
            else if (eventId == EventID.BATTLE_RESULT_COMPLETE)
            {
                var ev = Serialization.ToEvent<BattleResultCompleteEvent>(message);
                ev.Sender = owner;
                NetworkEvents.SendBattleResultComplete(ev);
            }
            else if (eventId == EventID.BATTLE_START_COMPLETE)
            {
                var ev = Serialization.ToEvent<BattleStartEvent>(message);
                ev.Sender = owner;
                NetworkEvents.SendBattleStart(ev);
            }

            // CLIENT EVENTS (coming from client to server)
            else if(eventId == EventID.JOIN)
            {
                var ev = Serialization.ToEvent<JoinWorldEvent>(message);
                ev.Sender = owner;
                NetworkEvents.SendJoinWorld(ev);
            }
            else if (eventId == EventID.PARTY_REQUEST_MOVE)
            {
                var ev = Serialization.ToEvent<MoveRequestEvent>(message);
                ev.Sender = owner;
                NetworkEvents.SendRequestEntityMove(ev);
            }
        } 
    }
}
