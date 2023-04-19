using Assets.Code.Assets.Code.Runtime.Tools;
using Assets.Code.Assets.Code.UIScreens.Base;
using Assets.Code.World;
using DG.Tweening;
using Game.Battle;
using Game.BattleActions;
using Game.Battler;
using Game.DataTypes;
using Game.Network.ServerPackets;
using System.Collections.Generic;
using System.Linq;
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
        private int _currentTurnNumber = 0;
        private BattleTurnLog _currentTurn;
        private BattleScreen _screen;

        private Dictionary<GameId, UnitView> _units = new Dictionary<GameId, UnitView>();
        private int _waitingUnits = 0;

        public BattlePlayback(BattleLogPacket log)
        {
            var header = log.DeserializeStartingState();
            _battle = new TurnBattle(header.BattleID, header.Attacker, header.Defender);
            _log = log;
        }

        public void StartPlayback()
        {
            var attackersObject = GameObject.Find("Team1");
            var unitIndex = 0;
            foreach (var teamSlot in SLOT_ORDER)
            {
                var battleUnit = unitIndex >= _battle.Attacker.Units.Length ? null : _battle.Attacker.Units[unitIndex];
                MainBehaviour.Destroy(attackersObject.transform.GetChild(teamSlot).GetChild(0).gameObject);
                unitIndex++;
                if (battleUnit == null) continue;
                AddUnit(battleUnit.UnitReference, attackersObject, teamSlot);
            }

            unitIndex = 0;
            var defendersObject = GameObject.Find("Team2");
            foreach (var teamSlot in SLOT_ORDER)
            {
                var battleUnit = unitIndex >= _battle.Defender.Units.Length ? null : _battle.Defender.Units[unitIndex];
                var container = defendersObject.transform.GetChild(teamSlot);
                MainBehaviour.Destroy(container.GetChild(0).gameObject);
                unitIndex++;
                if (battleUnit == null) continue;
                AddUnit(battleUnit.UnitReference, defendersObject, teamSlot);
            }
            _damageNumber = GameObject.Find("DamageNumber");
            _damageNumberPool.AddNew(_damageNumber);
            _damageNumberPool.Release(_damageNumber);

            _screen = ServiceContainer.Resolve<IScreenService>().Open<BattleScreen, BattleScreenSetup>(new BattleScreenSetup()
            {
                Attacker = _battle.Attacker,
                Defender = _battle.Defender,
                Units = _units
            });

            _ = PlayTurns();
        }

        private async Task WaitUnitsLoaded()
        {
            while (_waitingUnits > 0) await Task.Delay(100);
            await Task.Delay(1000);
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
            _screen.TakeDamage(view.Unit.Id, number);
            TextMeshPro text = null;
            var damageObject = _damageNumberPool.Obtain();
            if(damageObject == null)
            {
                damageObject = MainBehaviour.Instantiate(_damageNumber);
                _damageNumberPool.AddNew(damageObject);
            }

            text = damageObject.GetComponent<TextMeshPro>();
            text.text = number.ToString();
            text.transform.position = view.GameObject.transform.position + new Vector3(0, 0.5f, 0);
            damageObject.transform.LookAt(Camera.main.transform);
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
            var unitView = new UnitView(u);
            unitView.AddToScene(gameObject =>
            {
                _waitingUnits--;
                gameObject.transform.SetParent(teamTransform.transform.GetChild(index).transform);
                gameObject.transform.localPosition = Vector3.zero;
                gameObject.transform.localRotation = Quaternion.identity;
                _units[u.Id] = unitView;
                _ = MainBehaviour.RunAsync(() => unitView.UnitMonoBehaviour.PlayAnimation(UnitAnimation.BattleIddle), 0.1f);
            });
        }
    }
}
