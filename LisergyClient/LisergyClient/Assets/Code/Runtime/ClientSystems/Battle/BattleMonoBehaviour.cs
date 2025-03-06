using UnityEngine;

namespace Assets.Code
{
    // just for testing
    public class BattleMonoBehaviour : MonoBehaviour
    {
        private void TestBattle()
        {
            /*
            //// TEST ///
            StrategyGame game = new StrategyGame(TestSpecs.Generate(), null);
          
            ServiceContainer.Register<IAssetService, AssetService>(new AssetService());
            ServiceContainer.Register<IAudioService, AudioService>(new AudioService());
            ServiceContainer.Register<IScreenService, ScreenService>(new ScreenService());
            ServiceContainer.OnSceneLoaded();

            Serialization.LoadSerializers();
            MainBehaviour.ConfigureUnity();
            //// END TEST ////

            var enemyTeam = new BattleTeam(new Unit(2).SetBaseStats(), new Unit(1).SetBaseStats());
            var myTeam = new BattleTeam(new Unit(0).SetBaseStats(), new Unit(0).SetBaseStats());

            Guid id = Guid.Parse("699761ef-1119-4085-a93a-3e5eb1df0ddd");
            var battle = new TurnBattle(id, myTeam, enemyTeam);
            var log = new BattleLogPacket(battle);

            var autoRun = new AutoRun(battle);
            var result = autoRun.RunAllRounds();
            log.SetTurns(result);

            var header = log.DeserializeStartingState();
            var screen = ServiceContainer.Resolve<IScreenService>().Open<BattleScreen, BattleScreenSetup>(new BattleScreenSetup()
            {
                Attacker = header.Attacker,
                Defender = header.Defender,
                BattleId = id,
            });
            screen.PlayLog(log);
            */
        }

        void Start()
        {
            //TestBattle();
        }
    }
}
