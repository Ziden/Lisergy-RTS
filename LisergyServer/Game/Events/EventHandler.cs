
using Game.Events.ServerEvents;

namespace Game.Events
{

    public delegate void JoinEventHandler(JoinWorldEvent e);
    public delegate void AuthResultHandler(AuthResultEvent e);
    public delegate void TileVisibleHandler (TileVisibleEvent e);
    public delegate void SpecResponseHandler(GameSpecResponse e);

    public class EventSink
    {
        public static event JoinEventHandler OnJoinWorld;
        public static event AuthResultHandler OnPlayerAuth;
        public static event TileVisibleHandler OnTileVisible;
        public static event SpecResponseHandler OnSpecResponse;

        public static void SpecResponse(GameSpecResponse ev)
        {
            if (OnSpecResponse != null)
                OnSpecResponse(ev);
        }

        public static void TileVisible(TileVisibleEvent ev)
        {
            if (OnTileVisible != null)
                OnTileVisible(ev);
        }

        public static void AuthResult(AuthResultEvent ev)
        {
            if (OnPlayerAuth != null)
                OnPlayerAuth(ev);
        }

        public static void JoinWorld(JoinWorldEvent ev)
        {
            if (OnJoinWorld != null)
                OnJoinWorld(ev);
        }
    }
}
