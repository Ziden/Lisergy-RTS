using Assets.Code.ClientSystems.Party.UI;
using Cysharp.Threading.Tasks;
using Game.Engine.ECLS;
using Game.Systems.Battler;
using System;
using UnityEngine.UIElements;

namespace Party.UI
{
    public class WidgetPartyButton : WidgetElement
    {
        private VisualElement _greenHpBar;
        private VisualElement _hpBarContainer;
        private VisualElement _classIcon;
        private VisualElement _rarityCircle;
        private WidgetPartyButtonTracking _tracking;

        public IEntity PartyEntity;
      
        public Action OnClick;

        public WidgetPartyButton()
        {
            LoadUxmlFromResource("WidgetPartyButton");
            _rarityCircle = this.Q("RarityCircle").Required();
            _classIcon = this.Q("ClassIcon").Required();
            _hpBarContainer = this.Q("HpBarContainer").Required();
            _greenHpBar = this.Q("HpBar").Required();
            _hpBarContainer.style.display = DisplayStyle.None;
            RegisterCallback<PointerDownEvent>(e => OnClick?.Invoke(), TrickleDown.TrickleDown);
        }

        public void DisplayEntity(IEntity entity)
        {
            PartyEntity = entity;
            var group = entity.Get<BattleGroupComponent>();
            _ = DisplayUnit(group.Units.Leader);
            _tracking?.Track(entity);
        }

        public void AddEntityStateTracking()
        {
            this.Add(_tracking = new WidgetPartyButtonTracking());
        }

        public async UniTaskVoid DisplayUnit(Unit leader)
        {
            if (IsDestroyed()) return;
            if (_greenHpBar?.style.display == DisplayStyle.Flex)
            {
                var hpRatio = leader.HpRatio;
                _greenHpBar.style.width = Length.Percent(hpRatio * 100);
            }
            if (leader.Valid)
            {
                var spec = GameClient.Game.Specs.Units[leader.SpecId];
                var sprite = await GameClient.UnityServices().Assets.GetSprite(spec.IconArt);
                if (IsDestroyed()) return;
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

        public new class UxmlFactory : UxmlFactory<WidgetPartyButton, UxmlTraits> { }
        public new class UxmlTraits : VisualElement.UxmlTraits { }
    }
}