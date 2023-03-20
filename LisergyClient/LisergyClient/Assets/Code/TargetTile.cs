using Game;
using Game.Tile;
using System;

namespace Assets.Code
{
    public class TargetTile
    {
        private Notification _targetNotification;
        private Action<TileEntity> _action;

        public TargetTile(string msg, Action<TileEntity> onTarget)
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
