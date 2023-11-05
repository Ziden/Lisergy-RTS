using ClientSDK;
using Cysharp.Threading.Tasks;
using Game.Systems.Battler;
using Game.Systems.Party;
using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Code.Assets.Code.Runtime.UIScreens.Parts
{
    public class WidgetPartyButton
    {
        private VisualElement _root;
        private VisualElement _greenHpBar;
        private VisualElement _hpBarContainer;
        private VisualElement _rarityCircle;
        private VisualElement _classIcon;
        private IGameClient _client;

        public WidgetPartyButton(IGameClient client, VisualElement root)
        {
            _client = client;
            _root = root.Required();
            _hpBarContainer = root.Q("HpBar");
            _greenHpBar = _root.Q("GreenBar");
            _rarityCircle = _root.Q("RarityCircle").Required();
            _classIcon = _root.Q("ClassIcon").Required();
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
            var group = entity.Get<BattleGroupComponent>();
            _ = DisplayLeader(group.Units.Leader);
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
