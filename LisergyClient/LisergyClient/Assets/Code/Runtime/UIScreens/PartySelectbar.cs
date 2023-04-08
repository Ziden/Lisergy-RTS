using Assets.Code.Assets.Code.Assets;
using Assets.Code.Assets.Code.Runtime;
using Assets.Code.Assets.Code.UIScreens.Base;
using Assets.Code.Entity;
using Assets.Code.UI;
using Assets.Code.Views;
using Assets.Code.World;
using Game;
using Game.Events.Bus;
using Game.Movement;
using Game.Network.ClientPackets;
using Game.Party;
using Game.Pathfinder;
using Game.Tile;
using Game.World;
using GameAssets;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Code
{
    /// <summary>
    /// Bottom bar on the screen that allows the player to select a party
    /// </summary>
    public class PartySelectbar : UITKScreen, IEventListener
    {
        private VisualElement _cursor;
        private Button[] _partyButtons = new Button[4];
        private PathRenderer _pathRenderer;
        public override UIScreen ScreenAsset => UIScreen.PartySelectBar;

        public PartySelectbar()
        {
            _pathRenderer = new PathRenderer();
        }

        public override void OnOpen()
        {
            var service = ServiceContainer.Resolve<IScreenService>();
            _cursor = Root.Q<VisualElement>("PartySelector");
            for (var i = 0; i < 4; i++)
            {
                var index = i;
                _partyButtons[i] = Root.Q<Button>($"PartyPortrait-{i + 1}");
                _partyButtons[i].style.backgroundImage = null;
                _partyButtons[i].clicked += () => PartyButtonClick(index);
            }
            _cursor.style.display = DisplayStyle.None;
            EntityListener.OnPartyViewUpdated += OnPartyUpdated;
            ClientEvents.OnCameraMove += OnCameraMove;
            ClientEvents.OnClickTile += OnClickTile;
        }
        public override void OnClose()
        {
            EntityListener.OnPartyViewUpdated -= OnPartyUpdated;
            ClientEvents.OnCameraMove -= OnCameraMove;
            ClientEvents.OnClickTile -= OnClickTile;
        }

        private void OnCameraMove(Vector3 oldPos, Vector3 newPos)
        {
            ScreenService.Close<ActionsBar>();
            HidePartyInfo();
        }

        private void OnClickTile(TileEntity tile)
        {
            if (ScreenService.IsOpen<ActionsBar>()) ScreenService.Close<ActionsBar>();
            if (ClientState.SelectedParty != null)
            {
                ScreenService.Open<ActionsBar, ActionsBarSetup>(new ActionsBarSetup() { 
                    Party = ClientState.SelectedParty,
                    Tile = tile,
                    OnChosenAction = OnActionChosen
                });
            }
        }

        private void OnActionChosen(EntityAction action)
        {
            ScreenService.Close<ActionsBar>();
            PartyActions.PerformActionWithSelectedParty(action);
        }

        private void OnPartyUpdated(PartyView view)
        {
            if(view.Entity.IsMine() && view.IsPartyDeployed)
            {
                UpdatePartyIcon(view);
                if (ClientState.SelectedParty == null) SelectParty(view.Entity);
            }
        }

        public void UpdatePartyIcon(PartyView view)
        {
            var party = view.Entity;
            var units = party.BattleGroupLogic.GetValidUnits().ToList();
            var leader = units[0];
            ServiceContainer.Resolve<IAssetService>().GetSprite(leader.GetSpec().FaceArt, sprite =>
            {
                _partyButtons[party.PartyIndex].style.backgroundImage = new StyleBackground(sprite);
            });
        }

        private void PartyButtonClick(int partyIndex)
        {
            if (partyIndex >= MainBehaviour.LocalPlayer.Parties.Length) return;
            var party = MainBehaviour.LocalPlayer.Parties[partyIndex];
            if (party == null) return;
            SelectParty(party);
            CameraBehaviour.FocusOnTile(party.Tile);
        }

        private void SelectParty(PartyEntity party)
        {
            var button = _partyButtons[party.PartyIndex];
            _cursor.style.display = DisplayStyle.Flex;
            _cursor.style.left = button.worldBound.xMin - _cursor.parent.worldBound.xMin - 4;
            ClientEvents.SelectParty(party);
        }

        private void HidePartyInfo()
        {
            //UIManager.UnitPanel.Close();
        }

        private void ShowPartyInfo(PartyEntity party)
        {
            //UIManager.UnitPanel.ShowUnit(party.BattleGroupLogic.GetValidUnits().First());
        }
    }
}
