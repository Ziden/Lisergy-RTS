using Assets.Code.Assets.Code.UIScreens.Base;
using ClientSDK;
using ClientSDK.Data;
using Game.Battle;
using Game.Battle.Data;

namespace Assets.Code.Assets.Code.Runtime
{
    public interface INotificationService : IGameService { }

    public class NotificationService : INotificationService
    {
        private IScreenService _screen;
        private IGameClient _client;

        public NotificationService(IGameClient client)
        {
            _client = client;
        }

        public void OnSceneLoaded()
        {
            _screen = UnityServicesContainer.Resolve<IScreenService>();

            //UIEvents.OnBattleFinish += OnBattleFinish;
        }

        private void OnBattleFinish(BattleHeaderData h)
        {
            _screen.Open<BattleNotificationScreen>(new BattleNotificationSetup()
            {
                BattleHeader = h
            });
        }
    }
}
