using Assets.Code.Assets.Code.Runtime.UIScreens;
using Assets.Code.Assets.Code.Runtime.UIScreens.Parts;
using Assets.Code.Assets.Code.UIScreens.Base;
using Game;
using Game.Battle;
using Game.Events.Bus;
using GameAssets;
using System;
using UnityEngine.UIElements;

namespace Assets.Code
{
    public class BattleNotificationSetup : UIScreenSetup
    {
        public BattleHeader BattleHeader;
        public Action<BattleHeader> OnCheckItemDeltas;
    }

    public class BattleNotificationScreen : Notification, IEventListener
    {
        public override UIScreen ScreenAsset => UIScreen.BattleNotification;

        public override void OnOpen()
        {
            var popup = Root.Q();
            popup.style.top = 0;
            var setup = GetSetup<BattleNotificationSetup>();
            var win = IsWin(setup.BattleHeader);
            popup.Query(className: "outcome-loose").ForEach(e => e.style.display = win ? DisplayStyle.None : DisplayStyle.Flex);
            popup.Query(className: "outcome-win").ForEach(e => e.style.display = win ? DisplayStyle.Flex : DisplayStyle.None);

            var attacker = popup.Q<VisualElement>("PartyButton-Attacker");
            var t = attacker.GetType();
            var attackerLeader = setup.BattleHeader.Attacker.Leader.UnitReference;
            PartyButton.DisplayLeader(attacker, attackerLeader);


            var defender = popup.Q<VisualElement>("PartyButton-Defender-1");
            PartyButton.DisplayLeader(defender, setup.BattleHeader.Defender.Leader.UnitReference);
            base.OnOpen();
        }

        private bool IsWin(BattleHeader header)
        {
            return (
                header.Attacker.OwnerID == MainBehaviour.LocalPlayer.UserID && header.AttackerWins
                ||
                header.Defender.OwnerID == MainBehaviour.LocalPlayer.UserID && !header.AttackerWins
             );
        }

    }
}
