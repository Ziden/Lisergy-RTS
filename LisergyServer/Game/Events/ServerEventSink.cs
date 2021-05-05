
using Game.Events.ClientEvents;
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
    public delegate void BattleResultCompleteHandler(BattleResultEvent e);
    public delegate void BattleStartCompleteHandler(BattleStartEvent e);
    public delegate void BattleActionHandler(BattleActionEvent e);

    public class ServerEventSink
    {
        // TODO: Remove all static crap
        private static ServerEventSink _i;
        public ServerEventSink() {  _i = this; }

        private event JoinEventHandler _OnJoinWorld;
        private event AuthResultHandler _OnPlayerAuth;
        private event TileVisibleHandler _OnTileVisible;
        private event SpecResponseHandler _OnSpecResponse;
        private event EntityVisibleHandler _OnEntityVisible;
        private event EntityRequestMovementHandler _OnEntityRequestMove;
        private event EntityMoveHandler _OnEntityMove;
        private event MessagePopupHandler _OnMessagePopup;
        private event BattleResultCompleteHandler _OnBattleResult;
        private event BattleStartCompleteHandler _OnBattleStart;
        private event BattleActionHandler _OnBattleAction;

        public static event JoinEventHandler OnJoinWorld { add { _i._OnJoinWorld += value; } remove { _i._OnJoinWorld -= value; } }
        public static event AuthResultHandler OnPlayerAuth { add { _i._OnPlayerAuth += value; } remove { _i._OnPlayerAuth -= value; } }
        public static event TileVisibleHandler OnTileVisible { add { _i._OnTileVisible += value; } remove { _i._OnTileVisible -= value; } }
        public static event SpecResponseHandler OnSpecResponse { add { _i._OnSpecResponse += value; } remove { _i._OnSpecResponse -= value; } }
        public static event EntityVisibleHandler OnEntityVisible { add { _i._OnEntityVisible += value; } remove { _i._OnEntityVisible -= value; } }
        public static event EntityMoveHandler OnEntityMove { add { _i._OnEntityMove += value; } remove { _i._OnEntityMove -= value; } }
        public static event EntityRequestMovementHandler OnEntityRequestMove { add { _i._OnEntityRequestMove += value; } remove { _i._OnEntityRequestMove -= value; } }
        public static event MessagePopupHandler OnMessagePopup { add { _i._OnMessagePopup += value; } remove { _i._OnMessagePopup -= value; } }
        public static event BattleResultCompleteHandler OnBattleResult { add { _i._OnBattleResult += value; } remove { _i._OnBattleResult -= value; } }
        public static event BattleStartCompleteHandler OnBattleStart { add { _i._OnBattleStart += value; } remove { _i._OnBattleStart -= value; } }
        public static event BattleActionHandler OnBattleAction { add { _i._OnBattleAction += value; } remove { _i._OnBattleAction -= value; } }

        public static void SendEntityVisible(EntityVisibleEvent ev) => _i._OnEntityVisible?.Invoke(ev);
        public static void SendSpecResponse(GameSpecResponse ev) => _i._OnSpecResponse?.Invoke(ev);
        public static void SendTileVisible(TileVisibleEvent ev) => _i._OnTileVisible?.Invoke(ev);
        public static void SendAuthResult(AuthResultEvent ev) => _i._OnPlayerAuth?.Invoke(ev);
        public static void SendJoinWorld(JoinWorldEvent ev) => _i._OnJoinWorld?.Invoke(ev);
        public static void SendRequestEntityMove(MoveRequestEvent ev) => _i._OnEntityRequestMove?.Invoke(ev);
        public static void SendEntityMove(EntityMoveEvent ev) => _i._OnEntityMove?.Invoke(ev);
        public static void SendMessagePopup(MessagePopupEvent ev) => _i._OnMessagePopup?.Invoke(ev);
        public static void SendBattleResultComplete(BattleResultEvent ev) => _i._OnBattleResult?.Invoke(ev);
        public static void SendBattleStart(BattleStartEvent ev) => _i._OnBattleStart?.Invoke(ev);
        public static void SendBattleAction(BattleActionEvent ev) => _i._OnBattleAction?.Invoke(ev);
    }
}
