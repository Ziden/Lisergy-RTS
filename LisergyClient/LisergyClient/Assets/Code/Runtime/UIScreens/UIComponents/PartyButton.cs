using Assets.Code.Assets.Code.Assets;
using Assets.Code.Assets.Code.Runtime.PartyUnit;
using Game;
using Game.Systems.Battler;
using Game.Systems.Party;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Code.Assets.Code.Runtime.UIScreens.Parts
{
    public class PartyButton : VisualElement
    {
        public void DisplayParty(PartyEntity party)
        {
            //DisplayLeader(party.BattleGroupLogic.Leader);
        }

        public static void UpdateHealth(VisualElement healthBar, Unit unit)
        {
            var greenBar = healthBar.Q("GreenBar");
            greenBar.style.width = Length.Percent(unit.GetHpRatio() * 100); 
        }

        public static void HideHealth(VisualElement root)
        {
            var hpBar = root.Q("GreenBar");
            hpBar.style.display = DisplayStyle.None;
        }

        public static void DisplayLeader(VisualElement root, Unit leader)
        {
            var hpBar = root.Q("GreenBar");
            var rarityCircle = root.Q("RarityCircle");
            var classIcon = root.Q("ClassIcon");
            var hpRatio = leader.GetHpRatio();
            hpBar.style.width = Length.Percent(hpRatio * 100);
          
            /*
            var icon = leader.GetSpec().IconArt;
            ServiceContainer.Resolve<IAssetService>().GetSprite(icon, sprite =>
            {
                classIcon.style.backgroundImage = new StyleBackground(sprite);
            });
            */
        }

        public new class UxmlFactory : UxmlFactory<PartyButton, UxmlTraits> { }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
          
            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get { yield break; }
            }

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                var ate = ve as PartyButton;
            }
        }
    }
}
