
using Game.Events.ServerEvents;

namespace Game.Events
{
    public delegate void JoinEventHandler(JoinWorldEvent e);
    public delegate void AuthResultHandler(AuthResultEvent e);
    public delegate void TileVisibleHandler(TileVisibleEvent e);
    public delegate void SpecResponseHandler(GameSpecResponse e);
    public delegate void EntityVisibleHandler(EntityVisibleEvent e);
    public delegate void EntityRequestMovementHandler(MoveRequestEvent e);
    public delegate void EntityMoveHandler(EntityMoveEvent e);
    public delegate void MessagePopupHandler(MessagePopupEvent e);

    public class NetworkEvents
    {
        private static NetworkEvents _i;
        public NetworkEvents() {  _i = this; }

        public static event JoinEventHandler OnJoinWorld { add { _i._OnJoinWorld += value; } remove { _i._OnJoinWorld -= value; } }
        public static event AuthResultHandler OnPlayerAuth { add { _i._OnPlayerAuth += value; } remove { _i._OnPlayerAuth -= value; } }
        public static event TileVisibleHandler OnTileVisible { add { _i._OnTileVisible += value; } remove { _i._OnTileVisible -= value; } }
        public static event SpecResponseHandler OnSpecResponse { add { _i._OnSpecResponse += value; } remove { _i._OnSpecResponse -= value; } }
        public static event EntityVisibleHandler OnEntityVisible { add { _i._OnEntityVisible += value; } remove { _i._OnEntityVisible -= value; } }
        public static event EntityMoveHandler OnEntityMove { add { _i._OnEntityMove += value; } remove { _i._OnEntityMove -= value; } }
        public static event EntityRequestMovementHandler OnEntityRequestMove { add { _i._OnEntityRequestMove += value; } remove { _i._OnEntityRequestMove -= value; } }
        public static event MessagePopupHandler OnMessagePopup { add { _i._OnMessagePopup += value; } remove { _i._OnMessagePopup -= value; } }

        private event JoinEventHandler _OnJoinWorld;
        private event AuthResultHandler _OnPlayerAuth;
        private event TileVisibleHandler _OnTileVisible;
        private event SpecResponseHandler _OnSpecResponse;
        private event EntityVisibleHandler _OnEntityVisible;
        private event EntityRequestMovementHandler _OnEntityRequestMove;
        private event EntityMoveHandler _OnEntityMove;
        private event MessagePopupHandler _OnMessagePopup;

        public static void EntityVisible(EntityVisibleEvent ev) => _i._OnEntityVisible?.Invoke(ev);
        public static void SpecResponse(GameSpecResponse ev) => _i._OnSpecResponse?.Invoke(ev);
        public static void TileVisible(TileVisibleEvent ev) => _i._OnTileVisible?.Invoke(ev);
        public static void AuthResult(AuthResultEvent ev) => _i._OnPlayerAuth?.Invoke(ev);
        public static void JoinWorld(JoinWorldEvent ev) => _i._OnJoinWorld?.Invoke(ev);
        public static void RequestEntityMove(MoveRequestEvent ev) => _i._OnEntityRequestMove?.Invoke(ev);
        public static void EntityMove(EntityMoveEvent ev) => _i._OnEntityMove?.Invoke(ev);
        public static void MessagePopup(MessagePopupEvent ev) => _i._OnMessagePopup?.Invoke(ev);
    }
}
