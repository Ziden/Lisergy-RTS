using Assets.Code.Assets.Code.Assets;
using Assets.Code.Assets.Code.Runtime.UIScreens.Base;
using Assets.Code.Assets.Code.UIScreens.Base;
using Assets.Code.Battle;
using Assets.Code.World;
using Game.Battle;
using Game.Battle.BattleActions;
using Game.Battle.Data;
using Game.DataTypes;
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
    public class BattleScreenParam : IGameUiParam
    {
        public GameId BattleId;
        public BattleTeam Attacker;
        public BattleTeam Defender;
        public Action OnFinish;
    }

    public class BattleScreen : GameUi, IEventListener
    {
        public GameId BattleID { get; private set; }

        public event Action OnFinishedPlayback;

        public override UIScreen UiAsset => UIScreen.BattleScreen;

        private IDictionary<GameId, VisualElement> UnitHealthBars = new Dictionary<GameId, VisualElement>();
        private IDictionary<GameId, UnitView> Units => _battlePlayback.Units;
        private IAssetService _assets;
        private BattlePlayback _battlePlayback;
        private BattleLogPacket _log;
        private Camera _battleCamera;
       // public BattleHeaderData ResultHeader { get; private set; }

        private async Task AddHealthbar(UnitView view)
        {/*
            var hb = await _assets.GetScreen(UIScreen.HealthBar);
            var tree = hb.CloneTree();
            tree.style.position = Position.Absolute;
            Root.Add(tree);
            UnitHealthBars[view.Unit.Id] = tree;
            tree.MoveToEntity(Root.panel, view, _battleCamera);
            */
        }

        private async Task OnSceneLoaded()
        {
            var setup = GetParameter<BattleScreenParam>();
            OnFinishedPlayback += setup.OnFinish;
            _assets = UnityServicesContainer.Resolve<IAssetService>();
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

        /*
        public void SetResultHeader(BattleHeaderData header)
        {
            ResultHeader = header;
        }
        */

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
            var setup = GetParameter<BattleScreenParam>();
            BattleID = setup.BattleId;
            _battlePlayback = new BattlePlayback(setup.BattleId, setup.Attacker, setup.Defender);
            await _battlePlayback.SetupScene(scenario.transform, () => _ = OnSceneLoaded());
        }

        public override void OnOpen()
        {
            _uiService.Close<GameHUD>();
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
                unit.HP -= (byte)result.Damage;
            // PartyButton.UpdateHealth(UnitHealthBars[unit.Id], unit);
        }

        public void TakeDamage(GameId unitId, ushort damage)
        {
      
        }

        void ToggleBar(GameId unitId, bool visible) => UnitHealthBars[unitId].style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
    }
}
