using Assets.Code.Assets.Code.Assets;
using ClientSDK;
using Game;
using Game.Systems.Battler;
using Game.Systems.Party;
using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Code.Assets.Code.Runtime.UIScreens.Parts
{
    public class PartyButton
    {
        private VisualElement _root;
        private VisualElement _greenHpBar;
        private VisualElement _hpBarContainer;
        private VisualElement _rarityCircle;
        private VisualElement _classIcon;
        private IGameClient _client;

        public PartyButton(IGameClient client, VisualElement root)
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
            DisplayLeader(group.Units.Leader);
        }


        public void DisplayLeader(Unit leader)
        {
            if (_greenHpBar != null)
            {
                var hpRatio = leader.HpRatio;
                _greenHpBar.style.width = Length.Percent(hpRatio * 100);
            }

            if (leader.Valid)
            {
                var spec = _client.Game.Specs.Units[leader.SpecId];
                _client.UnityServices().Assets.GetSprite(spec.IconArt, sprite =>
                {
                    _classIcon.style.backgroundImage = new StyleBackground(sprite);
                });
            }
            else SetEmpty();
        }

        private void SetEmpty()
        {
            _classIcon.style.backgroundImage = null;
            if (_hpBarContainer != null)
            {
                _hpBarContainer.style.display = DisplayStyle.None;
            }
        }
    }
}
