using Assets.Code.Assets.Code.Assets;
using Assets.Code.Assets.Code.Runtime.PartyUnit;
using Game;
using Game.Battler;
using Game.Party;

using System.Collections.Generic;
using UnityEngine.UIElements;

namespace Assets.Code.Assets.Code.Runtime.UIScreens.Parts
{
    public class PartyButton : VisualElement
    {
        public void DisplayParty(PartyEntity party)
        {
            DisplayLeader(party.BattleGroupLogic.Leader);
        }

        public void DisplayLeader(Unit leader)
        {
            var hpBar = this.Q("GreenBar");
            var rarityCircle = this.Q("RarityCircle");
            var classIcon = this.Q("RarityCircle");
            hpBar.style.width = leader.GetHpRatio() * hpBar.style.maxWidth.value.value;
            ServiceContainer.Resolve<IAssetService>().GetSprite(leader.GetSpec().IconArt, sprite =>
            {
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
