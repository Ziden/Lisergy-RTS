using Assets.Code.Assets.Code.Runtime;
using Assets.Code.Assets.Code.UIScreens.Base;
using Assets.Code.UI;
using Chat.UI;
using Cysharp.Threading.Tasks;
using Game.Events.Bus;
using Game.Systems.Movement;
using Game.Systems.Party;
using Game.Tile;
using GameAssets;
using Player.UI;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Code
{
    /// <summary>
    /// Main in-game hud
    /// </summary>
    public class GameHUD : GameUi, IEventListener
    {
        private WidgetEntitySelectBar _selectBar;
        private WidgetChatSummary _chatSummary;

        public override UIScreen UiAsset => UIScreen.GameHud;

        public override void OnOpen()
        {
            var service = UnityServicesContainer.Resolve<IUiService>();
            _selectBar = Root.Q<WidgetEntitySelectBar>("WidgetEntitySelectBar").Required();
            _chatSummary = Root.Q<WidgetChatSummary>("WidgetChatSummary").Required();
            _selectBar.OnTownClicked += OnTownButtonClick;
            _selectBar.OnBuildClicked += () => OnBuildButton().Forget();
            ClientViewState.OnCameraMove += OnCameraMove;
            ClientViewState.OnSelectTile += OnClickTile;
            ClientViewState.OnSelectEntity += OnSelectEntity;
        }

        private void OnTownButtonClick()
        {
            var center = GameClient.Modules.Player.LocalPlayer.GetCenter();
            ClientViewState.SelectedEntityView = center.GetEntityView();
        }

        private async UniTaskVoid OnBuildButton()
        {
            GameClient.UnityServices().UI.Open<SelectScreen>(new SelectScreenParam()
            {
                ItemAsset = await GameClient.UnityServices().Assets.GetScreen(UIScreen.ScrollableIconButton),
                ToShow = GameClient.Modules.Buildings.GetBuildingsKnown().Select(b => new SelectableItem()
                {
                    Icon = b.Art,
                    Name = b.Name
                }).ToList()
            });
        }

        public override void OnLoaded(VisualElement root)
        {
            base.OnLoaded(root);
        }

        public override void OnClose()
        {
            ClientViewState.OnCameraMove -= OnCameraMove;
            ClientViewState.OnSelectTile -= OnClickTile;
            GameClient.ClientEvents.RemoveListener(this);
        }

        private void OnCameraMove(Vector3 newPos)
        {
            if (newPos != Vector3.zero)
            {
                UiService.Close<WidgetUnitDetails>();
                UiService.Close<ScreenPartyActions>();
                UiService.Close<ScreenEntityDetails>();
                UiService.Close<ScreenTileDetails>();
            }
        }

        private void OnSelectEntity(IUnityEntityView entity)
        {
            UiService.Open<ScreenEntityDetails>(new EntityDetailsParams() { Entity = entity.BaseEntity });
        }

        private void OnClickTile(TileEntity tile)
        {
            UiService.Close<WidgetUnitDetails>();
            UiService.Close<ScreenPartyActions>();
            UiService.Close<ScreenEntityDetails>();
            if (ClientViewState.SelectedEntityView != null && ClientViewState.SelectedEntityView.BaseEntity is PartyEntity party)
            {
                UiService.Open<ScreenPartyActions>(new ActionBarParams()
                {
                    Party = party,
                    Tile = tile,
                    OnChosenAction = OnActionChosen
                });
            }
        }

        private void OnActionChosen(EntityAction action)
        {
            UiService.Close<ScreenPartyActions>();
            PerformActionWithSelectedParty(action);
        }

        private void PerformActionWithSelectedParty(EntityAction action)
        {
            if (ClientViewState.SelectedEntityView.BaseEntity is PartyEntity party)
            {
                var tile = ClientViewState.SelectedTile;
                if (action == EntityAction.CHECK)
                {
                    var selectedTile = ClientViewState.SelectedTile;
                    if (selectedTile.Building != null)
                    {
                        UiService.Open<ScreenEntityDetails>(new EntityDetailsParams() { Entity = selectedTile.Building });
                    }
                    else if (selectedTile.EntitiesIn.Count > 0)
                    {
                        UiService.Open<ScreenEntityDetails>(new EntityDetailsParams() { Entity = selectedTile.EntitiesIn[0] });
                    }
                    return;
                }
                var intent = CourseIntent.Defensive;
                if (action == EntityAction.ATTACK) intent = CourseIntent.OffensiveTarget;
                else if (action == EntityAction.HARVEST) intent = CourseIntent.Harvest;
                GameClient.Modules.Actions.MoveParty(party, tile, intent);
            }
        }
    }
}
