using Assets.Code.Assets.Code.UIScreens.Base;
using Game.Battle;

namespace Assets.Code.Assets.Code.Runtime
{
    public interface INotificationService : IGameService { }

    public class NotificationService : INotificationService
    {
        private IScreenService _screen;

        public void OnSceneLoaded()
        {
            _screen = ServiceContainer.Resolve<IScreenService>();

            ClientEvents.OnBattleFinish += OnBattleFinish;
        }

        private void OnBattleFinish(CompleteBattleHeader h)
        {
            _screen.Open<BattleNotificationScreen, BattleNotificationSetup>(new BattleNotificationSetup()
            {
                BattleHeader = h
            });
        }
    }
}
