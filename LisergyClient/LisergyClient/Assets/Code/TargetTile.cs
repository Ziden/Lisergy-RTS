using Game;
using System;

namespace Assets.Code
{
    public class TargetTile
    {
        private Notification _targetNotification;
        private Action<Tile> _action;

        public TargetTile(string msg, Action<Tile> onTarget)
        {
            _action = onTarget;
            _targetNotification = UIManager.Notifications.ShowNotification(msg);
        }

        public void OnClick()
        {
            var tile = GameInput.GetTileMouseIs();
            if(tile != null)
            {
                _action(tile);
            }
            UIManager.Notifications.DisposeNotification(_targetNotification);
        }
    }
}
