using Assets.Code.Assets.Code.Assets;
using Assets.Code.Assets.Code.UIScreens.Base;
using Assets.Code.Battle;
using Game;
using Game.Battle;
using Game.BattleEvents;
using Game.Battler;
using Game.DataTypes;
using Game.Network.ServerPackets;
using GameDataTest;
using System;
using System.Linq;
using UnityEngine;

namespace Assets.Code
{
    public class BattleMonoBehaviour : MonoBehaviour
    {
        private BattlePlayback _playback;

        

        void Start()
        {
            //// TEST ///
            StrategyGame game = new StrategyGame(TestSpecs.Generate(), null);
            ServiceContainer.Register<IAssetService, AssetService>(new AssetService());
            var screenService = new ScreenService();
            ServiceContainer.Register<IScreenService, ScreenService>(screenService);
            screenService.OnSceneLoaded();
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

            _playback = new BattlePlayback(log);
            _playback.StartPlayback();
        }
    }
}
