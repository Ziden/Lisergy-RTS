using Assets.Code.Assets.Code.Assets;
using Assets.Code.Assets.Code.Runtime.PartyUnit;
using Game;
using Game.Battler;
using Game.Party;
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

        public static void DisplayLeader(VisualElement root, Unit leader)
        {
            var hpBar = root.Q("GreenBar");
            var rarityCircle = root.Q("RarityCircle");
            var classIcon = root.Q("ClassIcon");
            var hpRatio = leader.GetHpRatio();
            hpBar.style.width = Length.Percent(hpRatio * 100);
            var icon = leader.GetSpec().IconArt;
            Debug.Log($"Setting {icon.Address}");
            ServiceContainer.Resolve<IAssetService>().GetSprite(icon, sprite =>
            {
                Debug.Log("Setting bg for "+ sprite?.name);
                classIcon.style.backgroundImage = new StyleBackground(sprite);
            });
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
