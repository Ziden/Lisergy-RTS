using Assets.Code.Assets.Code.Audio;
using Assets.Code.Assets.Code.Runtime.Tools;
using Assets.Code.World;
using DG.Tweening;
using Game.Battle;
using Game.DataTypes;
using Game.Network.ServerPackets;
using Game.Systems.Battler;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Assets.Code.Battle
{
    public partial class BattlePlayback
    {
        private readonly static int[] SLOT_ORDER = new int[4] { 1, 2, 0, 3 };

        public TurnBattle Battle => _battle;
        private GameObject _damageNumber;
        private ActivationObjectPool _damageNumberPool = new ActivationObjectPool();
        private TurnBattle _battle;
        private BattleLogPacket _log;
        private IAudioService _audio;
        private int _currentTurnNumber = 0;
        private BattleTurnLog _currentTurn;
        public Camera Camera;
        public bool Playing { get; private set; }

        public Dictionary<GameId, UnitView> Units = new Dictionary<GameId, UnitView>();
        private int _waitingUnits = 0;

        public BattlePlayback(GameId battleId, BattleTeam attacker, BattleTeam defender)
        {
            //_battle = new TurnBattle(battleId, attacker, defender);
            _audio = UnityServicesContainer.Resolve<IAudioService>();
        }

        public async Task SetupScene(Transform root, Action onLoaded)
        {
            Debug.Log("Setting up Scene");
            var attackersObject = root.Find("Team1").gameObject;
            var unitIndex = 0;
            foreach (var teamSlot in SLOT_ORDER)
            {
                var battleUnit = unitIndex >= _battle.Attacker.Units.Length ? null : _battle.Attacker.Units[unitIndex];
                Main.Destroy(attackersObject.transform.GetChild(teamSlot).GetChild(0).gameObject);
                unitIndex++;
                if (battleUnit == null) continue;
                //AddUnit(battleUnit.UnitReference, attackersObject, teamSlot);
            }

            unitIndex = 0;
            var defendersObject = root.Find("Team2").gameObject;
            foreach (var teamSlot in SLOT_ORDER)
            {
                var battleUnit = unitIndex >= _battle.Defender.Units.Length ? null : _battle.Defender.Units[unitIndex];
                var container = defendersObject.transform.GetChild(teamSlot);
                Main.Destroy(container.GetChild(0).gameObject);
                unitIndex++;
                if (battleUnit == null) continue;
               // AddUnit(battleUnit.UnitReference, defendersObject, teamSlot);
            }
            _damageNumber = root.Find("DamageNumber").gameObject;
            await WaitUnitsLoaded();
            onLoaded();
        }

        public void PlayBattle(BattleLogPacket log)
        {
            Playing = true;
            _log = log;
            _ = PlayTurns();
            OnBattleFinish?.Invoke();
        }

        private async Task WaitUnitsLoaded()
        {
            while (_waitingUnits > 0) await Task.Delay(1);
            await Task.Delay(1);
        }

        private async Task PlayTurns()
        {
            await WaitUnitsLoaded();

            while(_currentTurnNumber < _log.Turns.Length)
            {
                _currentTurn = _log.Turns[_currentTurnNumber];
                await PlayTurn(_currentTurn);
                _currentTurnNumber++;
            }
        }

        private async Task PlayTurn(BattleTurnLog turn)
        {
            foreach(var ev in turn.Events)
            {
                await PlayEvent(ev);
            }
        }

        private void ShowDamage(UnitView view, ushort number)
        {
            TextMeshPro text = null;
            var damageObject = _damageNumberPool.Obtain();
            if(damageObject == null)
            {
                damageObject = Main.Instantiate(_damageNumber);
                damageObject.transform.localScale = new Vector3(0.02912701f, 0.02912701f, 0.02912701f);
                _damageNumberPool.AddNew(damageObject);
            }

            text = damageObject.GetComponent<TextMeshPro>();
            text.text = number.ToString();
            text.transform.position = view.GameObject.transform.position + new Vector3(0, 0.5f, 0);
            damageObject.transform.LookAt(Camera.transform);
            damageObject.transform.Rotate(0, 180, 0);
            var iniScale = text.transform.localScale;
            var durationSeconds = 2;
            text.transform.DOScale(iniScale / 2, durationSeconds).SetAutoKill(true).OnComplete(() => text.transform.localScale = iniScale);
            text.transform.DOJump(new Vector3(text.transform.position.x, text.transform.position.y + 0.1f, text.transform.position.z + 0.1f), 0.2f, 1, 2)
                .SetAutoKill(true).OnComplete(() => _damageNumberPool.Release(damageObject));
        }

        private void AddUnit(Unit u, GameObject teamTransform, int index)
        {
            _waitingUnits++;
            var unitView = new UnitView(null, u); // TODO ADD GAME CLIENT
            unitView.AddToScene(gameObject =>
            {
                _waitingUnits--;
                gameObject.transform.SetParent(teamTransform.transform.GetChild(index).transform);
                gameObject.transform.localPosition = Vector3.zero;
                gameObject.transform.localRotation = Quaternion.identity;
                Units[u.Id] = unitView;
                //_ = MainBehaviour.RunAsync(() => unitView.UnitMonoBehaviour.PlayAnimation(UnitAnimation.BattleIddle), 0.1f);
            });
        }
    }
}
