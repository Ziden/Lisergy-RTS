using Assets.Code.Assets.Code.Runtime;
using Assets.Code.ClientSystems.Party.UI;
using ClientSDK;
using ClientSDK.SDKEvents;
using ClientSDK.Services;
using Game.Engine.ECLS;
using Game.Engine.Events.Bus;
using Game.Entities;
using Game.Systems.Battler;
using Party.UI;
using System;
using UnityEngine.UIElements;

namespace Player.UI
{
    /// <summary>
    /// Bar to select parties and buildings on bottom of the game hud
    /// </summary>
    public class WidgetEntitySelectBar : WidgetElement, IEventListener
    {
        public event Action OnBuildClicked;
        public event Action OnTownClicked;

        private IPlayerModule _playerModule;
        private WidgetPartyButton[] _partyButtons = new WidgetPartyButton[4];
        private Button _townButton;
        private Button _buildButton;
        private VisualElement _buttonCursor;

        public WidgetEntitySelectBar()
        {
            LoadUxmlFromResource("WidgetEntitySelectBar");

            // Party Selection
            for (byte i = 0; i < 4; i++)
            {
                var index = i;
                _partyButtons[i] = this.Q<WidgetPartyButton>($"Party-{i + 1}").Required();
                _partyButtons[i].OnClick = () => PartyButtonClick(index);
                _partyButtons[i].AddEntityStateTracking();

            }
            _buttonCursor = this.Q<VisualElement>("PartySelector").Required();
            _buttonCursor.style.display = DisplayStyle.None;

            // Town/Build
            _buildButton = this.Q<Button>("BuildButton");
            _townButton = this.Q<Button>("TownButton");
            _townButton.clicked += TownButtonClick;
            _buildButton.clicked += BuildButtonClick;
        }

        public override void OnAddedDuringGame(IGameClient client)
        {
            _playerModule = client.Modules.Player;
            if (_playerModule.LocalPlayer == null) return;

            client.ClientEvents.On<OwnEntityInfoReceived>(this, OnEntityReceived);

            var parties = _playerModule.LocalPlayer.EntityLogic.GetParties();
            var buildings = _playerModule.LocalPlayer.EntityLogic.GetBuildings();

            for (byte i = 0; i < 4; i++)
            {
                if (i < parties.Count)
                {
                    var party = parties[i]; ;
                    _partyButtons[i].DisplayEntity(party);
                }
                else _partyButtons[i].SetEmpty();
            }
            if (ClientViewState.SelectedEntityView == null && parties.Count > 0)
            {
                SelectEntity(parties[0]);
            }
            if (buildings.Count == 0)
            {
                _townButton.style.display = DisplayStyle.None;
                _buildButton.style.display = DisplayStyle.Flex;
            }
            else
            {
                _townButton.style.display = DisplayStyle.Flex;
                _buildButton.style.display = DisplayStyle.None;
            }
        }

        public override void OnRemovedDuringGame(IGameClient client)
        {
            client.ClientEvents.RemoveListener(this);
        }

        private void OnEntityReceived(OwnEntityInfoReceived ev)
        {
            if (ev.Entity.EntityType == EntityType.Party)
            {
                if (ev.Entity.Get<BattleGroupComponent>().Units.Valids == 0) return;

                _partyButtons[0].DisplayEntity(ev.Entity);

                if (ClientViewState.SelectedEntityView == null)
                {
                    SelectEntity(ev.Entity);
                }
            }
            else
            {
                _townButton.style.visibility = Visibility.Visible;
            }
        }

        private void PartyButtonClick(int partyIndex)
        {
            var playerParties = _playerModule.LocalPlayer.EntityLogic.GetParties();
            var parties = _partyButtons[partyIndex];
            if (playerParties.Count <= partyIndex) return;
            var party = playerParties[partyIndex];
            if (party == null) return;
            SelectEntity(party);
        }

        private void BuildButtonClick()
        {
            PositionCursor(_buildButton.parent);
            OnBuildClicked?.Invoke();
        }

        private void TownButtonClick()
        {

            PositionCursor(_townButton.parent);
            OnTownClicked?.Invoke();
        }

        private void PositionCursor(VisualElement on)
        {
            _buttonCursor.style.display = DisplayStyle.Flex;
            _buttonCursor.style.left = on.worldBound.min.x;
            _buttonCursor.style.top = on.worldBound.min.y;
        }

        private void SelectEntity(IEntity party)
        {
            var button = _partyButtons[0]; // TODO:
            PositionCursor(button);
            var partyView = party.GetView();
            ClientViewState.SelectedEntityView = partyView;
        }

        public new class UxmlFactory : UxmlFactory<WidgetEntitySelectBar, UxmlTraits> { }
        public new class UxmlTraits : VisualElement.UxmlTraits { }
    }
}
