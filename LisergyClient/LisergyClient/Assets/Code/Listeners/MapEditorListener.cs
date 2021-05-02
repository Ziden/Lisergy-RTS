using Assets.Code.Listeners;
using Assets.Code.MapEditor;
using Assets.Code.World;
using Game;
using Game.Entity;
using Game.Events;
using Game.Events.ServerEvents;

namespace Assets.Code
{
    public class MapEditorListener: IListener
    {
        private ClientStrategyGame _game;
        private ClientTacticsMap _map;

        public MapEditorListener()
        {
            NetworkEvents.OnSpecResponse += ReceiveSpecs;
        }

        public void RegisterGameListeners()
        {
            NetworkEvents.OnMessagePopup += Message;
        }

        public void Message(MessagePopupEvent ev)
        {
            // TODO: Message factory
            if (ev.Type == PopupType.INVALID_COURSE)
                UIManager.Notifications.ShowNotification("Path has obstacles");
        }

        public void ReceiveSpecs(GameSpecResponse ev)
        {
            if (_game != null)
                throw new System.Exception("Received to register specs twice");
            var world = new ClientWorld(ev);
            _game = new ClientStrategyGame(ev.Spec, world);
            _map = new ClientTacticsMap(16, 16);
            RegisterGameListeners();
        }

        public void Update()
        {
            
        }
    }
}
