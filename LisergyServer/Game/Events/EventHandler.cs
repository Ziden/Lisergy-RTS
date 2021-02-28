
using Game.Events.ServerEvents;

namespace Game.Events
{
    public delegate void JoinEventHandler(JoinWorldEvent e);
    public delegate void AuthResultHandler(AuthResultEvent e);
    public delegate void TileVisibleHandler(TileVisibleEvent e);
    public delegate void SpecResponseHandler(GameSpecResponse e);
    public delegate void PartyVisibleHandler(PartyVisibleEvent e);
    public delegate void PartyRequestMoveHandler(MoveRequestEvent e);
    public delegate void PartyMoveEventHandler(PartyMoveEvent e);

    public class NetworkEvents
    {
        private static NetworkEvents _i;

        public NetworkEvents()
        {
            _i = this;
        }

        public static bool SERVER = true;

        public static event JoinEventHandler OnJoinWorld { add { _i._OnJoinWorld += value; } remove { _i._OnJoinWorld -= value; } }
        public static event AuthResultHandler OnPlayerAuth { add { _i._OnPlayerAuth += value; } remove { _i._OnPlayerAuth -= value; } }
        public static event TileVisibleHandler OnTileVisible { add { _i._OnTileVisible += value; } remove { _i._OnTileVisible -= value; } }
        public static event SpecResponseHandler OnSpecResponse { add { _i._OnSpecResponse += value; } remove { _i._OnSpecResponse -= value; } }
        public static event PartyVisibleHandler OnPartyVisible { add { _i._OnPartyVisible += value; } remove { _i._OnPartyVisible -= value; } }
        public static event PartyMoveEventHandler OnPartyMove { add { _i._OnPartyMove += value; } remove { _i._OnPartyMove -= value; } }
        public static event PartyRequestMoveHandler OnPartyRequestMove { add { _i._OnPartyRequestMove += value; } remove { _i._OnPartyRequestMove -= value; } }

        private event JoinEventHandler _OnJoinWorld;
        private event AuthResultHandler _OnPlayerAuth;
        private event TileVisibleHandler _OnTileVisible;
        private event SpecResponseHandler _OnSpecResponse;
        private event PartyVisibleHandler _OnPartyVisible;
        private event PartyRequestMoveHandler _OnPartyRequestMove;
        private event PartyMoveEventHandler _OnPartyMove;

        private static bool CanSend(GameEvent ev)
        {
            return ev.FromNetwork || (ev is ClientEvent && !SERVER || (ev is ServerEvent && SERVER));
        }

        public static void PartyVisible(PartyVisibleEvent ev)
        {
            if (_i._OnPartyVisible != null && CanSend(ev))
                _i._OnPartyVisible(ev);
        }

        public static void SpecResponse(GameSpecResponse ev)
        {
            if (_i._OnSpecResponse != null && CanSend(ev))
                _i._OnSpecResponse(ev);
        }

        public static void TileVisible(TileVisibleEvent ev)
        {
            if (_i._OnTileVisible != null && CanSend(ev))
                _i._OnTileVisible(ev);
        }

        public static void AuthResult(AuthResultEvent ev)
        {
            if (_i._OnPlayerAuth != null && CanSend(ev))
                _i._OnPlayerAuth(ev);
        }

        public static void JoinWorld(JoinWorldEvent ev)
        {
            if (_i._OnJoinWorld != null && CanSend(ev))
                _i._OnJoinWorld(ev);
        }

        public static void RequestPartyMove(MoveRequestEvent ev)
        {
            if (_i._OnPartyRequestMove != null && CanSend(ev))
                _i._OnPartyRequestMove(ev);
        }

        public static void PartyMove(PartyMoveEvent ev)
        {
            if (_i._OnPartyMove != null && CanSend(ev))
                _i._OnPartyMove(ev);
        }
    }
}
