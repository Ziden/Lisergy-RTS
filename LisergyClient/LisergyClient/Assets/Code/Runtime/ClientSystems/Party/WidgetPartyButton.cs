using ClientSDK;
using Cysharp.Threading.Tasks;
using Game.Systems.Battler;
using Game.Systems.Party;
using GameData;
using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Code.Assets.Code.Runtime.UIScreens.Parts
{
    public class WidgetPartyButton
    {
        private PartyEntity _party;
        private VisualElement _root;
        private VisualElement _greenHpBar;
        private VisualElement _hpBarContainer;
        private VisualElement _rarityCircle;
        private VisualElement _classIcon;
        private VisualElement _taskOverlay;
        private Label _taskNumber;
        private VisualElement _taskIcon;
        private IGameClient _client;

        public WidgetPartyButton(IGameClient client, VisualElement root)
        {
            _client = client;
            _root = root.Required();
            _hpBarContainer = root.Q("HpBar");
            _greenHpBar = _root.Q("GreenBar");
            _rarityCircle = _root.Q("RarityCircle").Required();
            _classIcon = _root.Q("ClassIcon").Required();
            _taskOverlay = _root.Q("TaskOverlay");
            if (_taskOverlay == null) return;
            _taskIcon = _taskOverlay.Q("TaskIcon").Required();
            _taskNumber = _taskOverlay.Q<Label>("TaskNumber").Required();
        }

        public Rect Bounds => _root.worldBound;

        public void UpdateHealth(float percentage)
        {
            _greenHpBar.style.width = Length.Percent(percentage * 100);
        }

        public void OnClick(Action a)
        {
            _root.RegisterCallback<PointerDownEvent>(e => a(), TrickleDown.TrickleDown);
        }

        public void HideHealth()
        {
            _greenHpBar.style.display = DisplayStyle.None;
        }

        public void DisplayParty(PartyEntity entity)
        {
            _party = entity;
            var group = entity.Get<BattleGroupComponent>();
            _ = DisplayLeader(group.Units.Leader);
        }

        public void UpdateResourceTask(HarvestingUpdateEvent ev)
        {
            if (ev.Depleted)
            {
                ClearTask();
                return;
            }
            _taskOverlay.style.display = DisplayStyle.Flex;
            var resourceSpec = _client.Game.Specs.Resources[ev.TileResources.Resource.ResourceId];
            _taskIcon.SetBackground(resourceSpec.Art);
            _taskNumber.text = ev.AmountHarvestedTotal.ToString();
        }

        public void ClearTask()
        {
            _taskOverlay.style.display = DisplayStyle.None;
        }

        public async UniTaskVoid DisplayLeader(Unit leader)
        {
            if (_greenHpBar != null)
            {
                var hpRatio = leader.HpRatio;
                _greenHpBar.style.width = Length.Percent(hpRatio * 100);
            }
            if (leader.Valid)
            {
                var spec = _client.Game.Specs.Units[leader.SpecId];
                var sprite = await _client.UnityServices().Assets.GetSprite(spec.IconArt);
                _classIcon.style.backgroundImage = new StyleBackground(sprite);
            }
            else SetEmpty();
        }

        public void SetEmpty()
        {
            _classIcon.style.backgroundImage = null;
            if (_hpBarContainer != null)
            {
                _hpBarContainer.style.display = DisplayStyle.None;
            }
        }
    }
}
