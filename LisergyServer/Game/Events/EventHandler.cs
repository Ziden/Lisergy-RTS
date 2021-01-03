
using Game.Events.ServerEvents;

namespace Game.Events
{

    public delegate void JoinEventHandler(JoinWorldEvent e);
    public delegate void AuthResultHandler(AuthResultEvent e);
    public delegate void TileVisibleHandler (TileVisibleEvent e);
    public delegate void SpecResponseHandler(GameSpecResponse e);
    public delegate void PartyVisibleHandler(PartyVisibleEvent e);

    public class EventSink
    {
        public static bool SERVER = true;

        public static event JoinEventHandler OnJoinWorld;
        public static event AuthResultHandler OnPlayerAuth;
        public static event TileVisibleHandler OnTileVisible;
        public static event SpecResponseHandler OnSpecResponse;
        public static event PartyVisibleHandler OnPartyVisible;

        private static bool CanSend(GameEvent ev)
        {
            return ev.FromNetwork || (ev is ClientEvent && !SERVER || (ev is ServerEvent && SERVER));
        }

        public static void PartyVisible(PartyVisibleEvent ev)
        {
            if (OnPartyVisible != null && CanSend(ev))
                OnPartyVisible(ev);
        }

        public static void SpecResponse(GameSpecResponse ev)
        {
            if (OnSpecResponse != null && CanSend(ev))
                OnSpecResponse(ev);
        }

        public static void TileVisible(TileVisibleEvent ev)
        {
            if (OnTileVisible != null && CanSend(ev))
                OnTileVisible(ev);
        }

        public static void AuthResult(AuthResultEvent ev)
        {
            if (OnPlayerAuth != null && CanSend(ev))
                OnPlayerAuth(ev);
        }

        public static void JoinWorld(JoinWorldEvent ev)
        {
            if (OnJoinWorld != null && CanSend(ev))
                OnJoinWorld(ev);
        }
    }
}
