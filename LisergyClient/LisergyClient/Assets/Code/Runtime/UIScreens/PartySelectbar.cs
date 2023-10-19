using Assets.Code.Assets.Code.Assets;
using Assets.Code.Assets.Code.Runtime;
using Assets.Code.Assets.Code.UIScreens.Base;
using Assets.Code.UI;
using Assets.Code.World;
using ClientSDK.SDKEvents;
using Game;
using Game.Events.Bus;
using Game.Systems.Battler;
using Game.Systems.Movement;
using Game.Systems.Party;
using Game.Tile;
using GameAssets;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Code
{
    /// <summary>
    /// Bottom bar on the screen that allows the player to select a party
    /// </summary>
    public class PartySelectbar : UITKScreen, IEventListener
    {
        private VisualElement _uiCursor;
        private Button[] _partyButtons = new Button[4];
        private Button _townButton;

        public override UIScreen ScreenAsset => UIScreen.PartySelectBar;

        public override void OnOpen()
        {
            var service = UnityServicesContainer.Resolve<IScreenService>();
            _townButton = Root.Q<Button>("TownButton");
            _townButton.clicked += () => TownButtonClick();
            _uiCursor = Root.Q<VisualElement>("PartySelector");
            for (var i = 0; i < 4; i++)
            {
                var index = i;
                _partyButtons[i] = Root.Q<Button>($"PartyPortrait-{i + 1}");
                _partyButtons[i].style.backgroundImage = null;
                _partyButtons[i].clicked += () => PartyButtonClick(index);
            }
            _uiCursor.style.display = DisplayStyle.None;
            UIEvents.OnCameraMove += OnCameraMove;
            UIEvents.OnClickTile += OnClickTile;
        }

        public override void OnLoaded(VisualElement root)
        {
            base.OnLoaded(root);
            GameClient.ClientEvents.Register<OwnEntityInfoReceived<PartyEntity>>(this, OnOwnPartyReceived);
        }

        public override void OnClose()
        {
            UIEvents.OnCameraMove -= OnCameraMove;
            UIEvents.OnClickTile -= OnClickTile;
        }

        private void OnCameraMove(Vector3 newPos)
        {
            if(newPos != Vector3.zero) ScreenService.Close<ActionsBar>();
        }

        private void OnClickTile(TileEntity tile)
        {
            if (ScreenService.IsOpen<ActionsBar>()) ScreenService.Close<ActionsBar>();
            if (ClientState.SelectedEntity != null && ClientState.SelectedEntity is PartyEntity party)
            {
                ScreenService.Open<ActionsBar, ActionsBarSetup>(new ActionsBarSetup()
                {
                    Party = party,
                    Tile = tile,
                    OnChosenAction = OnActionChosen
                });
            }
        }

        private void OnActionChosen(EntityAction action)
        {
            ScreenService.Close<ActionsBar>();
            PerformActionWithSelectedParty(action);
        }

        private void PerformActionWithSelectedParty(EntityAction action)
        {
            if(ClientState.SelectedEntity is PartyEntity party)
            {
                var tile = ClientState.SelectedTile;
                var intent = action == EntityAction.ATTACK ? CourseIntent.OffensiveTarget : CourseIntent.Defensive;
                GameClient.Modules.Actions.MoveParty(party, tile, intent);
            }
        }

        private void OnOwnPartyReceived(OwnEntityInfoReceived<PartyEntity> ev)
        {
            if (ev.Entity.Get<BattleGroupComponent>().Units.Valids == 0) return;
            UpdatePartyIcon(ev.Entity.GetEntityView());
            if (ClientState.SelectedEntity == null)
            {
                SelectEntity(ev.Entity);
            }
        }

        public void UpdatePartyIcon(PartyView view)
        {
            var party = view.Entity;
            var leader = party.Get<BattleGroupComponent>().Units.Leader;
            var leaderSpec = GameClient.Game.Specs.Units[leader.SpecId];
            UnityServicesContainer.Resolve<IAssetService>().GetSprite(leaderSpec.IconArt, sprite =>
            {
                _partyButtons[party.PartyIndex].style.backgroundImage = new StyleBackground(sprite);
            });
        }

        private void PartyButtonClick(int partyIndex)
        {
            var party = GameClient.Modules.Player.LocalPlayer.GetParty((byte)partyIndex);
            if (party == null) return;
            SelectEntity(party);
        }

        private void TownButtonClick()
        {
            _uiCursor.style.display = DisplayStyle.Flex;
            _uiCursor.style.left = _townButton.worldBound.xMin - _uiCursor.parent.worldBound.xMin - 12;
            var center = GameClient.Modules.Player.LocalPlayer.GetCenter();
            UIEvents.SelectEntity(center);
        }

        private void SelectEntity(PartyEntity party)
        {
            var button = _partyButtons[party.PartyIndex];
            _uiCursor.style.display = DisplayStyle.Flex;
            _uiCursor.style.left = button.worldBound.xMin - _uiCursor.parent.worldBound.xMin - 7;
            if(party != null) UIEvents.SelectEntity(party);
        }
    }
}
