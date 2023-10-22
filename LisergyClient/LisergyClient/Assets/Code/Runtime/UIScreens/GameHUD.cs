using Assets.Code.Assets.Code.Runtime;
using Assets.Code.Assets.Code.Runtime.UIScreens.Parts;
using Assets.Code.Assets.Code.UIScreens.Base;
using Assets.Code.UI;
using Assets.Code.World;
using ClientSDK.SDKEvents;
using Cysharp.Threading.Tasks;
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
    public class GameHUD : UITKScreen, IEventListener
    {
        private VisualElement _buttonCursor;
        private PartyButton[] _partyButtons = new PartyButton[4];
        private ChatSummary _chatSummary;
        private Button _townButton;

        public override UIScreen ScreenAsset => UIScreen.GameHud;

        public override void OnOpen()
        {
            var service = UnityServicesContainer.Resolve<IScreenService>();
            _townButton = Root.Q<Button>("TownButton");
            _townButton.clicked += () => TownButtonClick();
            _chatSummary = new ChatSummary(GameClient, Root.Q("Chat").Required());
            for (byte i = 0; i < 4; i++)
            {
                var index = i;
                _partyButtons[i] = new PartyButton(GameClient, Root.Q<VisualElement>($"Portrait-Container-{i + 1}"));
                _partyButtons[i].OnClick(() => PartyButtonClick(index));
                var party = GameClient.Modules.Player.LocalPlayer.GetParty(i);
                _partyButtons[i].DisplayParty(party);
            }
            _buttonCursor = Root.Q<VisualElement>("PartySelector");
            _buttonCursor.style.display = DisplayStyle.None;
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
                ScreenService.Close<UnitDetails>();
                ScreenService.Close<ActionsBar>();
                ScreenService.Close<EntityDetails>();
            }
        }

        private void OnClickTile(TileEntity tile)
        {
            ScreenService.Close<UnitDetails>();
            ScreenService.Close<ActionsBar>();
            ScreenService.Close<EntityDetails>();
            if (ClientState.SelectedEntity != null && ClientState.SelectedEntity is PartyEntity party)
            {
                ScreenService.Open<ActionsBar>(new ActionBarParams()
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
                if(action == EntityAction.CHECK)
                {
                    var selectedTile = ClientState.SelectedTile;
                    if(selectedTile.Building != null)
                    {
                        ScreenService.Open<EntityDetails>(new EntityDetailsParams()
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
            var party = GameClient.Modules.Player.LocalPlayer.GetParty((byte)partyIndex);
            if (party == null) return;
            SelectEntity(party);
        }

        private void TownButtonClick()
        {
            _buttonCursor.style.display = DisplayStyle.Flex;
            _buttonCursor.style.left = _townButton.worldBound.xMin - _buttonCursor.parent.worldBound.xMin - 11;
            var center = GameClient.Modules.Player.LocalPlayer.GetCenter();
            ClientState.SelectedEntity = center;
        }

        private void SelectEntity(PartyEntity party)
        {
            var button = _partyButtons[party.PartyIndex];
            _buttonCursor.style.display = DisplayStyle.Flex;
            _buttonCursor.style.left = button.Bounds.xMin - _buttonCursor.parent.worldBound.xMin + 6;
            if(party != null) ClientState.SelectedEntity = party;
        }
    }
}
