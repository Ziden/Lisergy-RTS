using Game;
using Game.Systems.Battler;
using Game.Systems.Party;
using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace Assets.Code.Assets.Code.Runtime.UIScreens.Parts
{
    public class PartyButton 
    {
        private Button _btn;
        private VisualElement _root;
        private VisualElement _hpBar;
        private VisualElement _rarityCircle;
        private VisualElement _classIcon;

        public PartyButton(VisualElement root)
        {
            _root = root.Required();
            _btn = (Button)_root;
            _hpBar = _root.Q("GreenBar").Required();
            _rarityCircle = _root.Q("RarityCircle").Required();
            _classIcon = _root.Q("ClassIcon").Required();
        }

        public void UpdateHealth(float percentage)
        {
            _hpBar.style.width = Length.Percent(percentage * 100); 
        }

        public void OnClick(Action a)
        {
            _btn.clicked += a;
        }

        public void HideHealth()
        {
            _hpBar.style.display = DisplayStyle.None;
        }

        public void DisplayLeader(Unit leader)
        {
            var hpRatio = leader.HpRatio;
            _hpBar.style.width = Length.Percent(hpRatio * 100);
          
            /*
            var icon = leader.GetSpec().IconArt;
            ServiceContainer.Resolve<IAssetService>().GetSprite(icon, sprite =>
            {
                classIcon.style.backgroundImage = new StyleBackground(sprite);
            });
            */
        }
    }
}
