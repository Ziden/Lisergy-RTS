using Assets.Code.Assets.Code.Assets;
using Assets.Code.Assets.Code.Runtime.UIScreens.Base;
using Assets.Code.Assets.Code.Runtime.UIScreens.Parts;
using Assets.Code.Assets.Code.UIScreens.Base;
using Assets.Code.Battle;
using Assets.Code.Views;
using Assets.Code.World;
using Game.Battle;
using Game.BattleActions;
using Game.Battler;
using Game.DataTypes;
using Game.Events;
using Game.Events.Bus;
using Game.Network.ServerPackets;
using GameAssets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace Assets.Code
{
    public class BattleScreenSetup : UIScreenSetup
    {
        public GameId BattleId;
        public BattleTeam Attacker;
        public BattleTeam Defender;
        public Action OnFinish;
    }

    public class BattleScreen : UITKScreen, IEventListener
    {
        public GameId BattleID { get; private set; }

        public event Action OnFinishedPlayback;

        public override UIScreen ScreenAsset => UIScreen.BattleScreen;

        private IDictionary<GameId, VisualElement> UnitHealthBars = new Dictionary<GameId, VisualElement>();
        private IDictionary<GameId, UnitView> Units => _battlePlayback.Units;
        private IAssetService _assets;
        private BattlePlayback _battlePlayback;
        private BattleLogPacket _log;
        private Camera _battleCamera;
        public CompleteBattleHeader ResultHeader { get; private set; }

        private async Task AddHealthbar(UnitView view)
        {
            await _assets.GetScreen(UIScreen.HealthBar, hb =>
            {
                var tree = hb.CloneTree();
                tree.style.position = Position.Absolute;
                Root.Add(tree);
                UnitHealthBars[view.Unit.Id] = tree;
                tree.MoveToEntity(Root.panel, view, _battleCamera);
            });
        }

        private async Task OnSceneLoaded()
        {
            var setup = GetSetup<BattleScreenSetup>();
            OnFinishedPlayback += setup.OnFinish;
            _assets = ServiceContainer.Resolve<IAssetService>();
            for (var i = 0; i < 4; i++)
            {
                if (i < setup.Attacker.Units.Length) await AddHealthbar(Units[setup.Attacker.Units[i].UnitID]);
                if (i < setup.Defender.Units.Length) await AddHealthbar(Units[setup.Defender.Units[i].UnitID]);
            }
            _battlePlayback.OnBattleFinish += () => OnFinishedPlayback?.Invoke();
            _battlePlayback.OnUnitAct += OnUnitAct;
            _battlePlayback.OnUnitFinishAct += OnUnitFinishAct;
            _battlePlayback.OnAttacked += OnAttack;
            _battlePlayback.Camera = _battleCamera;

            if (Camera.main != null) Camera.main.enabled = false;

            _battleCamera.enabled = true;

            Debug.Log("Battle Scene Loaded");
            if (_log != null)
            {
                _battlePlayback.PlayBattle(_log);
            }
        }

        public void SetResultHeader(CompleteBattleHeader header)
        {
            ResultHeader = header;
        }

        private async Task PrepareBattleSceneAsync()
        {
            if (!SceneManager.GetAllScenes().Any(s => s.name == "Battle"))
            {
                var op = SceneManager.LoadSceneAsync("Battle", LoadSceneMode.Additive);
                while (!op.isDone) await Task.Delay(1);
            }

            var battleScene = SceneManager.GetAllScenes().FirstOrDefault(s => s.name == "Battle");
            var scenario = battleScene.GetRootGameObjects().FirstOrDefault(o => o.name == "BattleScenario");
            _battleCamera = scenario.transform.Find("BattleCamera").gameObject.GetComponent<Camera>();
            var setup = GetSetup<BattleScreenSetup>();
            BattleID = setup.BattleId;
            _battlePlayback = new BattlePlayback(setup.BattleId, setup.Attacker, setup.Defender);
            _ = _battlePlayback.SetupScene(scenario.transform, () => OnSceneLoaded());
        }

        public override void OnOpen()
        {
            _screenService.Close<PartySelectbar>();
            _ = PrepareBattleSceneAsync();

        }

        public void PlayLog(BattleLogPacket log)
        {

            _log = log;
            if(_battlePlayback != null && !_battlePlayback.Playing)
            {
                _battlePlayback.PlayBattle(_log);
            }
        }

        private void OnUnitAct(BattleUnit u) => ToggleBar(u.UnitID, false);
        private void OnUnitFinishAct(BattleUnit u) => ToggleBar(u.UnitID, true);

        private void OnAttack(AttackAction atk)
        {
            var unit = Units[atk.Defender.UnitID].Unit;
            var result = atk.Result as AttackActionResult;
            if (unit.HP - result.Damage <= 0)
                unit.HP = 0;
            else
                unit.HP -= result.Damage;
            PartyButton.UpdateHealth(UnitHealthBars[unit.Id], unit);
        }

        public void TakeDamage(GameId unitId, ushort damage)
        {
      
        }

        void ToggleBar(GameId unitId, bool visible) => UnitHealthBars[unitId].style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
    }
}
