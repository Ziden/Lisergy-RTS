using Assets.Code.Assets.Code.Runtime;
using Assets.Code.Assets.Code.Runtime.UIScreens.Parts;
using Assets.Code.Assets.Code.UIScreens.Base;
using Assets.Code.UI;
using Assets.Code.World;
using ClientSDK.SDKEvents;
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
    public class GameHUD : GameUi, IEventListener
    {
        private VisualElement _buttonCursor;
        private PartyButton[] _partyButtons = new PartyButton[4];
        private ChatSummary _chatSummary;
        private Button _townButton;

        public override UIScreen UiAsset => UIScreen.GameHud;

        public override void OnOpen()
        {
            var service = UnityServicesContainer.Resolve<IUiService>();
            _townButton = Root.Q<Button>("TownButton");
            _townButton.clicked += () => TownButtonClick();
            _chatSummary = new ChatSummary(GameClient, Root.Q("Chat").Required());
            for (byte i = 0; i < 4; i++)
            {
                var index = i;
                _partyButtons[i] = new PartyButton(GameClient, Root.Q<VisualElement>($"Portrait-Container-{i + 1}"));
                _partyButtons[i].OnClick(() => PartyButtonClick(index));
                if (i < GameClient.Modules.Player.LocalPlayer.Parties.Count)
                {
                    var party = GameClient.Modules.Player.LocalPlayer.Parties[i];
                    _partyButtons[i].DisplayParty(party);
                }
                else _partyButtons[i].SetEmpty();
            }
            _buttonCursor = Root.Q<VisualElement>("PartySelector");
            _buttonCursor.style.display = DisplayStyle.None;
            if (ClientState.SelectedEntity == null && GameClient.Modules.Player.LocalPlayer.Parties.Count > 0)
            {
                SelectEntity(GameClient.Modules.Player.LocalPlayer.Parties[0]);
            }
            GameClient.ClientEvents.Register<OwnEntityInfoReceived<PartyEntity>>(this, OnOwnPartyReceived);
            ClientState.OnCameraMove += OnCameraMove;
            ClientState.OnSelectTile += OnClickTile;
        }

        public override void OnLoaded(VisualElement root)
        {
            base.OnLoaded(root);
        }

        public override void OnClose()
        {
            ClientState.OnCameraMove -= OnCameraMove;
            ClientState.OnSelectTile -= OnClickTile;
            GameClient.ClientEvents.RemoveListener(this);
            _chatSummary.Dispose();
        }

        private void OnCameraMove(Vector3 newPos)
        {
            if (newPos != Vector3.zero)
            {
                UiService.Close<UnitDetails>();
                UiService.Close<ActionsBar>();
                UiService.Close<EntityDetails>();
            }
        }

        private void OnClickTile(TileEntity tile)
        {
            UiService.Close<UnitDetails>();
            UiService.Close<ActionsBar>();
            UiService.Close<EntityDetails>();
            if (ClientState.SelectedEntity != null && ClientState.SelectedEntity.BaseEntity is PartyEntity party)
            {
                UiService.Open<ActionsBar>(new ActionBarParams()
                {
                    Party = party,
                    Tile = tile,
                    OnChosenAction = OnActionChosen
                });
            }
        }

        private void OnActionChosen(EntityAction action)
        {
            UiService.Close<ActionsBar>();
            PerformActionWithSelectedParty(action);
        }

        private void PerformActionWithSelectedParty(EntityAction action)
        {
            if(ClientState.SelectedEntity.BaseEntity is PartyEntity party)
            {
                var tile = ClientState.SelectedTile;
                if(action == EntityAction.CHECK)
                {
                    var selectedTile = ClientState.SelectedTile;
                    if(selectedTile.Building != null)
                    {
                        UiService.Open<EntityDetails>(new EntityDetailsParams()
                        {
                            Entity = selectedTile.Building
                        });
                    }
                    return;
                }
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
            _partyButtons[party.PartyIndex].DisplayLeader(leader);
        }

        private void PartyButtonClick(int partyIndex)
        {
            if(GameClient.Modules.Player.LocalPlayer.Parties.Count <= partyIndex) return;
            var party = GameClient.Modules.Player.LocalPlayer.Parties[partyIndex];
            if (party == null) return;
            SelectEntity(party);
        }

        private void TownButtonClick()
        {
            _buttonCursor.style.display = DisplayStyle.Flex;
            _buttonCursor.style.left = _townButton.worldBound.xMin - _buttonCursor.parent.worldBound.xMin - 11;
            var center = GameClient.Modules.Player.LocalPlayer.GetCenter();
            ClientState.SelectedEntity = center.GetEntityView();
        }

        private void SelectEntity(PartyEntity party)
        {
            var button = _partyButtons[party.PartyIndex];
            _buttonCursor.style.display = DisplayStyle.Flex;
            _buttonCursor.style.left = button.Bounds.xMin - _buttonCursor.parent.worldBound.xMin + 6;
            ClientState.SelectedEntity = party.GetEntityView();
        }
    }
}
