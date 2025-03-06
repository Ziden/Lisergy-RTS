using Assets.Code.Assets.Code.Runtime.UIScreens;
using ClientSDK;
using ClientSDK.Data;
using System;
using System.Collections.Generic;

namespace Assets.Code.Assets.Code.Runtime
{
    /// <summary>
    /// For displaying the little popups on the screen
    /// </summary>
    public interface INotificationService : IGameService
    {

        /// <summary>
        /// Display the notification on the screen for a couple seconds.
        /// Might add the notification to the queue if screen is occupied
        /// </summary>
        void Display<T>(object param = null) where T : Notification;
    }

    public class NotificationService : INotificationService
    {
        private IGameClient _client;
        private Queue<Action> _notifications = new Queue<Action>();
        private Notification _open;

        public NotificationService(IGameClient client)
        {
            _client = client;
        }

        public void Display<T>(object param = null) where T : Notification
        {
            if (_notifications.Count == 0)
            {
                OpenNotificationPopup<T>(param);
            }
            else _notifications.Enqueue(() => OpenNotificationPopup<T>(param));
        }

        private void OpenNotificationPopup<T>(object param = null) where T : Notification
        {
            _open = _client.UnityServices().UI.Open<T>(param);
            _ = CloseTask();
        }

        private async UniTaskVoid CloseTask()
        {
            await UniTask.Delay(4000);
            _client.UnityServices().UI.Close(_open);
            _open = null;
            if (_notifications.TryDequeue(out var next))
            {
                await UniTask.Delay(1000);
                next();
            }
        }

        public void OnSceneLoaded()
        {
        }
    }
}
